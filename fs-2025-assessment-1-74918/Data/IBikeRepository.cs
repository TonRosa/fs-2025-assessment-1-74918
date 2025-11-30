using fs_2025_a_api_demo_002.Models;

namespace fs_2025_a_api_demo_002.Data
{
    public record BikeQuery(
       string? Status = null,
       bool? AvailableOnly = null,
       int? MinAvailableBikes = null,
       string? Search = null,
       string? SortBy = null,
       string? SortDir = "asc",
       int Page = 1,
       int PageSize = 20);

    public record BikeSummary(int TotalStations, int StationsWithBikes, int TotalAvailableBikes);

    public interface IBikeRepository
    {

        public record BikeQuery(
            string? Status = null,
            bool? AvailableOnly = null,
            int? MinAvailableBikes = null,
            string? Search = null,
            string? SortBy = null,
            string? SortDir = "asc",
            int Page = 1,
            int PageSize = 20);

        public record BikeSummary(int TotalStations, int StationsWithBikes, int TotalAvailableBikes);

        public interface IBikeRepository
        {
            Task<IEnumerable<Bike>> QueryAsync(BikeQuery query);
            Task<Bike?> GetByNumberAsync(int number);
            Task<BikeSummary> GetSummaryAsync();
            Task UpdateRandomAsync();
        }
    }
}
