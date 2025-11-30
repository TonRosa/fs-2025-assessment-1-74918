using fs_2025_a_api_demo_002.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace fs_2025_a_api_demo_002.Services
{
    public interface IDataService
    {
        Task<List<Bike>> GetAllStationsAsync();
    }
}