using Azure;
using fs_2025_a_api_demo_002.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace fs_2025_a_api_demo_002.Data;
public class CosmosBikeRepository : IBikeRepository
{
    private readonly Container _container;

    public CosmosBikeRepository(CosmosClient client, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        var db = config["Cosmos:Database"] ?? "Dublin";
        var container = config["Cosmos:Container"] ?? "bikes";
        _container = client.GetContainer(db, container);
        
    }

    public async Task<IEnumerable<Bike>> QueryAsync(BikeQuery query)
    {
        var sql = "SELECT * FROM c";
        var iterator = _container.GetItemQueryIterator<Bike>(new QueryDefinition(sql));
        var results = new List<Bike>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.Resource);
        }

        var items = results.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.Status))
            items = items.Where(b => string.Equals(b.Status, query.Status, StringComparison.OrdinalIgnoreCase));

        if (query.AvailableOnly == true)
            items = items.Where(b => b.AvailableBikes > 0);

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

        return items;
    }

    public async Task<Bike?> GetByNumberAsync(int number)
    {
        try
        {
            var response = await _container.ReadItemAsync<Bike>(number.ToString(), new PartitionKey(number));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<BikeSummary> GetSummaryAsync()
    {
        var sql = "SELECT VALUE COUNT(1) FROM c";
        var it = _container.GetItemQueryIterator<int>(new QueryDefinition(sql));
        int total = 0;
        if (it.HasMoreResults)
        {
            var r = await it.ReadNextAsync();
            total = r.Resource.FirstOrDefault();
        }

        var all = (await QueryAsync(new BikeQuery(Page: 1, PageSize: 10000))).ToList();
        var totalAvailable = all.Sum(b => b.AvailableBikes);
        var withBikes = all.Count(b => b.AvailableBikes > 0);
        return new BikeSummary(total, withBikes, totalAvailable);
    }

    public Task UpdateRandomAsync() => Task.CompletedTask;


    public async Task<Bike?> AddStationAsync(Bike bike)
    {
        var response = await _container.CreateItemAsync(bike, new PartitionKey(bike.Number));
        return response.Resource;
    }
}
