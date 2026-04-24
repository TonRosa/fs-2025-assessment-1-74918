using System.Text.Json.Serialization;

namespace DublinBikes.BlazorClient.Models
{
    public class StationDto
    {
        [JsonPropertyName("number")]
        public int number { get; set; }

        [JsonPropertyName("contract_name")]
        public string contract_name { get; set; } = "";

        [JsonPropertyName("name")]
        public string name { get; set; } = "";

        [JsonPropertyName("address")]
        public string address { get; set; } = "";

        [JsonPropertyName("banking")]
        public bool banking { get; set; }

        [JsonPropertyName("bonus")]
        public bool bonus { get; set; }

        [JsonPropertyName("bike_stands")]
        public int bike_stands { get; set; }

        [JsonPropertyName("available_bike_stands")]
        public int available_bike_stands { get; set; }

        [JsonPropertyName("available_bikes")]
        public int available_bikes { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; } = "OPEN";

        [JsonPropertyName("last_update")]
        public long last_update { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; } = "";
    }
}