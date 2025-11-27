using fs_2025_a_api_demo_002.Data;

namespace fs_2025_a_api_demo_002.Endpoints
{
    public static class BikeEndPoints
    {
        public static void AddBikeEndPoints(this WebApplication app)
        {
            // List with optional filters: ?status=OPEN&available=true
            app.MapGet("/bikes", (BikeData bikeData, string? status, bool? available) =>
            {
                var items = bikeData.Bikes.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(status))
                    items = items.Where(b => string.Equals(b.status, status, StringComparison.OrdinalIgnoreCase));

                if (available == true)
                    items = items.Where(b => b.available_bikes > 0);

                return Results.Ok(items);
            });

            // Get station by number
            app.MapGet("/bikes/{number:int}", (BikeData bikeData, int number) =>
            {
                var station = bikeData.Bikes.FirstOrDefault(b => b.number == number);
                return station is null ? Results.NotFound() : Results.Ok(station);
            });

            // API v1 group
            var v1 = app.MapGroup("/api/v1");
            v1.MapGet("bikes", (BikeData bikeData) => Results.Ok(bikeData.Bikes));
            v1.MapGet("bikes/{number:int}", (BikeData bikeData, int number) =>
            {
                var station = bikeData.Bikes.FirstOrDefault(b => b.number == number);
                return station is null ? Results.NotFound() : Results.Ok(station);
            });
        }
    }

}
