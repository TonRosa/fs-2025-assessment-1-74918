using fs_2025_a_api_demo_002.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fs_2025_a_api_demo_002.Data
{


    //public class JsonBikeRepository : IBikeRepository
    //{
    //    private readonly BikeData _data;
    //    private readonly Random _rng = new();

    //    public JsonBikeRepository(BikeData data) => _data = data;

    //    public Task<IEnumerable<Bike>> QueryAsync(BikeQuery query)
    //    {
    //        var items = _data.Bikes.AsEnumerable();

    //        if (!string.IsNullOrWhiteSpace(query.Status))
    //            items = items.Where(b => string.Equals(b.status, query.Status, StringComparison.OrdinalIgnoreCase));

    //        if (query.AvailableOnly == true)
    //            items = items.Where(b => b.available_bikes > 0);

    //        if (query.MinAvailableBikes.HasValue)
    //            items = items.Where(b => b.available_bikes >= query.MinAvailableBikes.Value);

    //        if (!string.IsNullOrWhiteSpace(query.Search))
    //        {
    //            var s = query.Search.Trim();
    //            items = items.Where(b =>
    //                (b.name ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase) ||
    //                (b.address ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase));
    //        }

    //        items = (query.SortBy?.ToLowerInvariant()) switch
    //        {
    //            "available_bikes" => query.SortDir == "desc" ? items.OrderByDescending(b => b.available_bikes) : items.OrderBy(b => b.available_bikes),
    //            "available_bike_stands" => query.SortDir == "desc" ? items.OrderByDescending(b => b.available_bike_stands) : items.OrderBy(b => b.available_bike_stands),
    //            "name" => query.SortDir == "desc" ? items.OrderByDescending(b => b.name) : items.OrderBy(b => b.name),
    //            "number" or _ => query.SortDir == "desc" ? items.OrderByDescending(b => b.number) : items.OrderBy(b => b.number),
    //        };

    //        var page = Math.Max(1, query.Page);
    //        var pageSize = Math.Clamp(query.PageSize, 1, 200);
    //        items = items.Skip((page - 1) * pageSize).Take(pageSize);

    //        return Task.FromResult(items);
    //    }

    //    public Task<Bike?> GetByNumberAsync(int number) =>
    //        Task.FromResult(_data.Bikes.FirstOrDefault(b => b.number == number));

    //    public Task<BikeSummary> GetSummaryAsync()
    //    {
    //        var total = _data.Bikes.Count;
    //        var withBikes = _data.Bikes.Count(b => b.available_bikes > 0);
    //        var totalAvailable = _data.Bikes.Sum(b => b.available_bikes);
    //        return Task.FromResult(new BikeSummary(total, withBikes, totalAvailable));
    //    }

    //    public Task UpdateRandomAsync()
    //    {
    //        foreach (var b in _data.Bikes)
    //        {
    //            var change = _rng.Next(-2, 3);
    //            var newVal = Math.Clamp(b.available_bikes + change, 0, b.bike_stands);
    //            b.available_bikes = newVal;
    //            b.available_bike_stands = b.bike_stands - newVal;
    //            b.last_update = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    //        }
    //        return Task.CompletedTask;
    //    }
    //}

    public class JsonBikeRepository : IBikeRepository
    {
        private readonly BikeData _data;
        private readonly Random _rng = new();

        public JsonBikeRepository(BikeData data) => _data = data;

        public Task<IEnumerable<Bike>> QueryAsync(BikeQuery query)
        {
            var items = _data.Bikes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query.Status))
                items = items.Where(b => string.Equals(b.Status, query.Status, StringComparison.OrdinalIgnoreCase));

            if (query.AvailableOnly == true)
                items = items.Where(b => b.AvailableBikes > 0);

            if (query.MinAvailableBikes.HasValue)
                items = items.Where(b => b.AvailableBikes >= query.MinAvailableBikes.Value);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                items = items.Where(b =>
                    (b.Name ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    (b.Address ?? string.Empty).Contains(s, StringComparison.OrdinalIgnoreCase));
            }

            items = (query.SortBy?.ToLowerInvariant()) switch
            {
                "available_bikes" => query.SortDir == "desc" ? items.OrderByDescending(b => b.AvailableBikes) : items.OrderBy(b => b.AvailableBikes),
                "available_bike_stands" => query.SortDir == "desc" ? items.OrderByDescending(b => b.AvailableBikeStands) : items.OrderBy(b => b.AvailableBikeStands),
                "name" => query.SortDir == "desc" ? items.OrderByDescending(b => b.Name) : items.OrderBy(b => b.Name),
                "number" or _ => query.SortDir == "desc" ? items.OrderByDescending(b => b.Number) : items.OrderBy(b => b.Number),
            };

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, 200);
            items = items.Skip((page - 1) * pageSize).Take(pageSize);

            return Task.FromResult(items);
        }

        public Task<Bike?> GetByNumberAsync(int number) =>
            Task.FromResult(_data.Bikes.FirstOrDefault(b => b.Number == number));

        public Task<BikeSummary> GetSummaryAsync()
        {
            var total = _data.Bikes.Count;
            var withBikes = _data.Bikes.Count(b => b.AvailableBikes > 0);
            var totalAvailable = _data.Bikes.Sum(b => b.AvailableBikes);
            return Task.FromResult(new BikeSummary(total, withBikes, totalAvailable));
        }

        public Task UpdateRandomAsync()
        {
            foreach (var b in _data.Bikes)
            {
                var change = _rng.Next(-2, 3);
                var newVal = Math.Clamp(b.AvailableBikes + change, 0, b.BikeStands);
                b.AvailableBikes = newVal;
                b.AvailableBikeStands = b.BikeStands - newVal;
                b.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            return Task.CompletedTask;
        }
    }
}
