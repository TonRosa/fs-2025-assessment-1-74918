using fs_2025_a_api_demo_002.Data;
using fs_2025_a_api_demo_002.Services;
using Microsoft.AspNetCore.Mvc;

namespace fs_2025_a_api_demo_002.Endpoints
{
   
    public static class BikeEndPoints
    {
        public static void AddBikeEndPoints(this WebApplication app)
        {
            // List with optional filters: ?status=OPEN&available=true
            app.MapGet("/bikes", ([FromServices] BikeData bikeData, [FromQuery] string? status, [FromQuery] bool? available) =>
            {
                var items = bikeData.Bikes.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(status))
                    items = items.Where(b => string.Equals(b.Status, status, StringComparison.OrdinalIgnoreCase));

                if (available == true)
                    items = items.Where(b => b.AvailableBikes > 0);

                return Results.Ok(items);
            });

            app.MapGet("/summary", async ([FromServices] BikeData bikeData, [FromQuery] string ? status, [FromQuery] bool ? available) =>
            {
                IDataService _data = (IDataService)bikeData;
                var summary = await _data.GetSummaryAsync();
                return Results.Ok(summary);
            });

            // Get station by number
            app.MapGet("/bikes/{number:int}", ([FromServices] BikeData bikeData, int number) =>
            {
                var station = bikeData.Bikes.FirstOrDefault(b => b.Number == number);
                return station is null ? Results.NotFound() : Results.Ok(station);
            });

            // API v1 group
            var v1 = app.MapGroup("/api/v1");
            v1.MapGet("bikes", ([FromServices] BikeData bikeData) => Results.Ok(bikeData.Bikes));
            v1.MapGet("bikes/{number:int}", ([FromServices] BikeData bikeData, int number) =>
            {
                var station = bikeData.Bikes.FirstOrDefault(b => b.Number == number);
                return station is null ? Results.NotFound() : Results.Ok(station);
            });
        }
    }
}
