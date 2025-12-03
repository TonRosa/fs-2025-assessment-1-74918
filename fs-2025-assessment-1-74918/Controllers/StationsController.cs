using fs_2025_a_api_demo_002.Data;
using fs_2025_a_api_demo_002.Models;
using fs_2025_a_api_demo_002.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/stations")]
    public class StationsController : ControllerBase
    {
        protected readonly JsonDataService _data;
        protected readonly IMemoryCache _cache;

        public StationsController(JsonDataService data, IMemoryCache cache)
        {
            _data = data;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            string? search, string? status, string? sort, int page = 1, int pageSize = 20)
        {
            var stations = await _data.GetAllStationsAsync();

            // Filtering
            if (!string.IsNullOrEmpty(search))
                stations = stations.Where(s => s.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(status))
                stations = stations.Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

            // Sorting
            stations = sort switch
            {
                "name" => stations.OrderBy(s => s.Name).ToList(),
                "-name" => stations.OrderByDescending(s => s.Name).ToList(),
                "availableBikes" => stations.OrderBy(s => s.AvailableBikes).ToList(),
                "-availableBikes" => stations.OrderByDescending(s => s.AvailableBikes).ToList(),
                _ => stations
            };

            // Paging
            var paged = stations.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(paged);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var stations = await _data.GetAllStationsAsync();

            var summary = new
            {
                totalStations = stations.Count,
                totalBikesAvailable = stations.Sum(s => s.AvailableBikes),
                averageBikes = stations.Average(s => s.AvailableBikes),
                maxAvailable = stations.OrderByDescending(s => s.AvailableBikes).First()
            };

            return Ok(summary);
        }

        [HttpGet("Station/Number{number}")]
        public async Task<IActionResult> GetStationByNumber(int number)
        {
            var station = await _data.GetStationAsync(number);

                        return Ok(station);
        }
     
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Station station)
        {
                    var newStation = await _data.AddStationAsync(station);

            return  Ok(newStation);

        }
        [HttpPut("{number}")]
        public async Task<IActionResult> Put(int number, [FromBody] Station station)
        {
            var currentStation = await _data.UpdateStationAsync(station);



            return Ok(currentStation);
        }
    }

}
