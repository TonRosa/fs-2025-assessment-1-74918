using fs_2025_a_api_demo_002.Data;
using fs_2025_a_api_demo_002.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.Json;

namespace fs_2025_a_api_demo_002.Services
{

    public class JsonDataService : IDataService
    {
        private readonly List<Station> _stations = new();
        private readonly IMemoryCache _cache;
        private readonly ILogger<JsonDataService> _logger;
        private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);

        public JsonDataService(IMemoryCache cache, IWebHostEnvironment env, ILogger<JsonDataService> logger)
        {
            _cache = cache;
            _logger = logger;
            var dataPath = Path.Combine(env.ContentRootPath, "Data", "dublinbike.json");
            try
            {
                if (File.Exists(dataPath))
                {
                    var json = File.ReadAllText(dataPath);
                    var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var items = JsonSerializer.Deserialize<List<Station>>(json, opts);
                    if (items != null)
                        _stations.AddRange(items);
                }
                else
                {
                    _logger.LogWarning("Data file not found: {Path}", dataPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load stations from {Path}", dataPath);
            }
        }

        private string BuildCacheKey(string? status, int? minBikes, string? q, string? sort, string? dir, int page, int pageSize)
        {
            q = q?.Trim()?.ToLowerInvariant();
            return $"stations:query:status={status ?? ""}:minBikes={minBikes?.ToString() ?? ""}:q={q ?? ""}:sort={sort ?? ""}:dir={dir ?? ""}:page={page}:pageSize={pageSize}";
        }

        private void TrackKey(string key) => _cacheKeys.TryAdd(key, 0);

        private void InvalidateQueryCache()
        {
            foreach (var key in _cacheKeys.Keys)
            {
                _cache.Remove(key);
            }
            _cacheKeys.Clear();
        }

        public Task<IReadOnlyList<Station>> QueryStationsAsync(string? status = null, int? minBikes = null, string? q = null, string? sort = null, string? dir = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var cacheKey = BuildCacheKey(status, minBikes, q, sort, dir, page, pageSize);

            if (_cache.TryGetValue<IReadOnlyList<Station>>(cacheKey, out var cached))
            {
                return Task.FromResult(cached);
            }

            // perform in-memory filtering/sorting/paging on snapshot
            var items = _stations.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(status))
                items = items.Where(s => string.Equals(s.Status, status, StringComparison.OrdinalIgnoreCase));

            if (minBikes.HasValue)
                items = items.Where(s => s.AvailableBikes >= minBikes.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qnorm = q.Trim();
                items = items.Where(s =>
                    (!string.IsNullOrEmpty(s.Name) && s.Name.Contains(qnorm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Address) && s.Address.Contains(qnorm, StringComparison.OrdinalIgnoreCase)));
            }

            items = (sort ?? "").ToLowerInvariant() switch
            {
                "name" => dir == "desc" ? items.OrderByDescending(s => s.Name) : items.OrderBy(s => s.Name),
                "availablebikes" => dir == "desc" ? items.OrderByDescending(s => s.AvailableBikes) : items.OrderBy(s => s.AvailableBikes),
                "occupancy" => dir == "desc" ? items.OrderByDescending(s => s.Occupancy) : items.OrderBy(s => s.Occupancy),
                _ => items
            };

            var paged = items.Skip((page - 1) * pageSize).Take(pageSize).ToList().AsReadOnly();

            // cache result
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheTtl
            };
            _cache.Set(cacheKey, paged, cacheEntryOptions);
            TrackKey(cacheKey);

            return Task.FromResult<IReadOnlyList<Station>>(paged);
        }

        public Task<Station?> GetStationAsync(int number)
        {
            // cache single station with short key
            var key = $"stations:single:{number}";
            if (_cache.TryGetValue<Station?>(key, out var cached)) return Task.FromResult(cached);

            var station = _stations.FirstOrDefault(s => s.Number == number);
            _cache.Set(key, station, _cacheTtl);
            TrackKey(key);
            return Task.FromResult(station);
        }

        public Task<Dictionary<string, object>> GetSummaryAsync()
        {
            var key = "stations:summary";
            if (_cache.TryGetValue<Dictionary<string, object>>(key, out var cached)) return Task.FromResult(cached);

            var totalStations = _stations.Count;
            var totalBikeStands = _stations.Sum(s => s.BikeStands);
            var totalAvailableBikes = _stations.Sum(s => s.AvailableBikes);
            var countsByStatus = _stations
                .GroupBy(s => s.Status ?? "UNKNOWN", StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Count());

            var summary = new Dictionary<string, object>
            {
                ["totalStations"] = totalStations,
                ["totalBikeStands"] = totalBikeStands,
                ["totalAvailableBikes"] = totalAvailableBikes,
                ["countsByStatus"] = countsByStatus,
                ["generatedAtUtc"] = DateTimeOffset.UtcNow
            };

            _cache.Set(key, summary, _cacheTtl);
            TrackKey(key);
            return Task.FromResult(summary);
        }

        public Task<Station> AddStationAsync(Station station)
        {
            // Ensure unique number
            var max = _stations.Count == 0 ? 0 : _stations.Max(s => s.Number);
            if (station.Number == 0) station.Number = max + 1;
            _stations.Add(station);

            // invalidate query cache
            InvalidateQueryCache();

            return Task.FromResult(station);
        }

        public Task<bool> UpdateStationAsync(Station station)
        {
            var idx = _stations.FindIndex(s => s.Number == station.Number);
            if (idx < 0) return Task.FromResult(false);

            _stations[idx] = station;

            // invalidate cache so subsequent queries reflect updated data
            InvalidateQueryCache();

            return Task.FromResult(true);
        }

        // Return all stations for background updater (keeps object references so updates persist)
        public Task<List<Station>> GetAllStationsAsync()
        {
            // return a shallow copy of the list; elements are the same instances stored internally
            return Task.FromResult(_stations.ToList());
        }
    }
}
