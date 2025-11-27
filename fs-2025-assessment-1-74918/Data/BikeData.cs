using fs_2025_a_api_demo_002.Models;
using System.Text.Json;

namespace fs_2025_a_api_demo_002.Data
{
    public class BikeData
    {
        public List<Bike> Bikes { get; private set; } = new List<Bike>();

        public BikeData()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string filePath = Path.Combine(AppContext.BaseDirectory, "Data", "dublinbike.json");
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                Bikes = JsonSerializer.Deserialize<List<Bike>>(jsonData, options) ?? new List<Bike>();
            }
        }
    }
}
