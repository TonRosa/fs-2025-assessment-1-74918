using System.Text.Json.Serialization;

namespace fs_2025_a_api_demo_002.Models
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

        // JSON contains epoch milliseconds
        [JsonPropertyName("last_update")]
        public long LastUpdateEpochMs { get; set; }

        //[JsonIgnore]
        public DateTimeOffset LastUpdateUtc
            => DateTimeOffset.FromUnixTimeMilliseconds(LastUpdateEpochMs);

        //[JsonIgnore]
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
                    // Fallback to UTC if timezone id not found on the host
                    return LastUpdateUtc;
                }
            }
        }

      
        public double Occupancy
        {
            get => BikeStands > 0 ? (double)AvailableBikes / BikeStands : 0.0;
        }
    }
}
