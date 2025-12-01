using fs_2025_a_api_demo_002.Data;
using fs_2025_a_api_demo_002.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace fs_2025_a_api_demo_002.Services
{

    public class CosmosDataService : IDataService
    {
        private readonly CosmosBikeRepository _repo;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);
        private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();

        public CosmosDataService(CosmosBikeRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        private void TrackKey(string key) => _cacheKeys.TryAdd(key, 0);

        private void InvalidateQueryCache()
        {
            foreach (var k in _cacheKeys.Keys)
                _cache.Remove(k);
            _cacheKeys.Clear();
        }

        private static Station MapBikeToStation(Bike b)
            => new Station
            {
                Number = b.Number,
                Name = b.Name,
                Address = b.Address,
                Position = b.Position is null ? null : new GeoPosition
                {
                    Latitude = b.Position.Lat,
                    Longitude = b.Position.Lng
                },
                BikeStands = b.BikeStands,
                AvailableBikes = b.AvailableBikes,
                AvailableBikeStands = b.AvailableBikeStands,
                Status = b.Status,
                LastUpdateEpochMs = b.LastUpdate
            };

        private string BuildCacheKey(string? status, int? minBikes, string? q, string? sort, string? dir, int page, int pageSize)
        {
            q = q?.Trim()?.ToLowerInvariant();
            return $"cosmos:stations:status={status ?? ""}:minBikes={minBikes?.ToString() ?? ""}:q={q ?? ""}:sort={sort ?? ""}:dir={dir ?? ""}:page={page}:pageSize={pageSize}";
        }

        public async Task<IReadOnlyList<Station>> QueryStationsAsync(string? status = null, int? minBikes = null, string? q = null, string? sort = null, string? dir = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var cacheKey = BuildCacheKey(status, minBikes, q, sort, dir, page, pageSize);
            if (_cache.TryGetValue<IReadOnlyList<Station>>(cacheKey, out var cached)) return cached;

            var bikes = (await _repo.QueryAsync(new BikeQuery(Page: 1, PageSize: 10000))).ToList();
            var items = bikes.Select(MapBikeToStation).AsEnumerable();

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

            _cache.Set(cacheKey, paged, _cacheTtl);
            TrackKey(cacheKey);

            return paged;
        }

        public async Task<Station?> GetStationAsync(int number)
        {
            var key = $"cosmos:station:{number}";
            if (_cache.TryGetValue<Station?>(key, out var cached)) return cached;

            var bike = await _repo.GetByNumberAsync(number);
            var station = bike is null ? null : MapBikeToStation(bike);
            _cache.Set(key, station, _cacheTtl);
            TrackKey(key);
            return station;
        }

        public async Task<Dictionary<string, object>> GetSummaryAsync()
        {
            var key = "cosmos:stations:summary";
            if (_cache.TryGetValue<Dictionary<string, object>>(key, out var cached)) return cached;

            var bikes = (await _repo.QueryAsync(new BikeQuery(Page: 1, PageSize: 10000))).ToList();
            var stations = bikes.Select(MapBikeToStation).ToList();

            var totalStations = stations.Count;
            var totalBikeStands = stations.Sum(s => s.BikeStands);
            var totalAvailableBikes = stations.Sum(s => s.AvailableBikes);
            var countsByStatus = stations
                .GroupBy(s => s.Status ?? "UNKNOWN", StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Count());

            var summary = new Dictionary<string, object>
            {
                ["totalStations"] = totalStations,
                ["totalBikeStands"] = totalBikeStands,
                ["totalAvailableBikes"] = totalAvailableBikes,
                ["countsByStatus"] = countsByStatus,
                ["generatedAtUtc"] = DateTimeOffset.UtcNow,
                ["dataSource"] = "Cosmos"
            };

            _cache.Set(key, summary, _cacheTtl);
            TrackKey(key);

            return summary;
        }

        // Write operations are not supported by the sample Cosmos repository.
        public Task<Station> AddStationAsync(Station station)
        {
            throw new NotSupportedException("AddStationAsync is not supported by CosmosDataService. Implement repository write methods to enable this.");
        }

        public Task<bool> UpdateStationAsync(Station station)
        {
            throw new NotSupportedException("UpdateStationAsync is not supported by CosmosDataService. Implement repository write methods to enable this.");
        }

        // Return all stations (mapped) for background/updater use
        public async Task<List<Station>> GetAllStationsAsync()
        {
            var bikes = (await _repo.QueryAsync(new BikeQuery(Page: 1, PageSize: 10000))).ToList();
            return bikes.Select(MapBikeToStation).ToList();
        }
    }

}
