using fs_2025_a_api_demo_002.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


namespace fs_2025_a_api_demo_002.Controllers
{
    [Route("api/v{version:apiVersion}/stations")]
    //[Route("api/[controller]")]
    public abstract class StationsControllerBase : ControllerBase
    {
        protected readonly IDataService _data;
        protected readonly IMemoryCache _cache;

        protected StationsControllerBase(IDataService data, IMemoryCache cache)
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
    }

}
