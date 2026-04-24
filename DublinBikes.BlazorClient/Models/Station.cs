using System.Text.Json.Serialization;

namespace DublinBikes.BlazorClient.Models
{
    public class GeoPosition
    {
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [JsonPropertyName("lng")]
        public double Longitude { get; set; }
    }

    public class Station
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("position")]
        public GeoPosition? Position { get; set; }

        [JsonPropertyName("bike_stands")]
        public int BikeStands { get; set; }

        [JsonPropertyName("available_bikes")]
        public int AvailableBikes { get; set; }

        [JsonPropertyName("available_bike_stands")]
        public int AvailableBikeStands { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("last_update")]
        public long LastUpdateEpochMs { get; set; }

        public DateTimeOffset LastUpdateUtc =>
            DateTimeOffset.FromUnixTimeMilliseconds(LastUpdateEpochMs);

        public DateTimeOffset LastUpdateLocal
        {
            get
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Dublin");
                    return TimeZoneInfo.ConvertTime(LastUpdateUtc, tz);
                }
                catch
                {
                    return LastUpdateUtc;
                }
            }
        }

        public double Occupancy =>
            BikeStands > 0 ? (double)AvailableBikes / BikeStands : 0.0;
    }

    public class StationSummary
    {
        [JsonPropertyName("totalStations")]
        public int TotalStations { get; set; }

        [JsonPropertyName("totalBikeStands")]
        public int TotalBikeStands { get; set; }

        [JsonPropertyName("totalAvailableBikes")]
        public int TotalAvailableBikes { get; set; }

        [JsonPropertyName("countsByStatus")]
        public Dictionary<string, int>? CountsByStatus { get; set; }
    }
}