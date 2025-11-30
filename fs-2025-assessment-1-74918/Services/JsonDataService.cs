using fs_2025_a_api_demo_002.Data;
using fs_2025_a_api_demo_002.Models;
using System.Text.Json;

namespace fs_2025_a_api_demo_002.Services
{
    //public class JsonDataService : IDataService
    //{
    //    private readonly List<Bike> _stations;

    //    public JsonDataService()
    //    {
    //        var json = File.ReadAllText("Data/dublinbikes.json");
    //        _stations = JsonSerializer.Deserialize<List<Bike>>(json);
    //    }

    //    public Task<List<Bike>> GetAllStationsAsync()
    //        => Task.FromResult(_stations);
    //}

    public class JsonDataService : IDataService
    {
        private readonly BikeData _bikeData;

        public JsonDataService(BikeData bikeData)
        {
            _bikeData = bikeData;
        }

        public Task<List<Bike>> GetAllStationsAsync()
        {
            // BikeData.Bikes is List<Bike>
            return Task.FromResult(_bikeData.Bikes);
        }
    }
}
