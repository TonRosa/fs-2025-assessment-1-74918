using fs_2025_a_api_demo_002.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace fs_2025_a_api_demo_002.Services
{
    public interface IDataService
    {
        Task<IReadOnlyList<Station>> QueryStationsAsync(
            string? status = null,
            int? minBikes = null,
            string? q = null,
            string? sort = null,
            string? dir = null,
            int page = 1,
            int pageSize = 20);

        Task<Station?> GetStationAsync(int number);

        Task<Dictionary<string, object>> GetSummaryAsync();

        Task<Station> AddStationAsync(Station station);

        Task<bool> UpdateStationAsync(Station station);

        // Provide a way for background services to obtain all stations for updates
        Task<List<Station>> GetAllStationsAsync();
    }
}