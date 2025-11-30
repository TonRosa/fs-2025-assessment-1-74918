using fs_2025_a_api_demo_002.Data;
using fs_2025_a_api_demo_002.Models;
using Microsoft.Azure.Cosmos;

namespace fs_2025_a_api_demo_002.Services
{
    //public class CosmosDataService : IDataService
    //{
    //    private readonly CosmosClient _client;
    //    private readonly Container _container;

    //    public CosmosDataService(IConfiguration config)
    //    {
    //        _client = new CosmosClient(config["Cosmos:Endpoint"], config["Cosmos:Key"]);
    //        _container = _client.GetContainer("DublinBikesDb", "Stations");
    //    }

    //    public async Task<List<Bike>> GetAllStationsAsync()
    //    {
    //        var query = _container.GetItemQueryIterator<Bike>("SELECT * FROM c");
    //        var results = new List<Bike>();

    //        while (query.HasMoreResults)
    //            results.AddRange(await query.ReadNextAsync());

    //        return results;
    //    }
    //}

    public class CosmosDataService : IDataService
    {
        private readonly CosmosBikeRepository _repo;

        public CosmosDataService(CosmosBikeRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Bike>> GetAllStationsAsync()
        {
            // Query with a large page size to fetch all entries for the sample
            var items = await _repo.QueryAsync(new BikeQuery(Page: 1, PageSize: 10000));
            return items.ToList();
        }
    }


}
