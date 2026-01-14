namespace DublinBikes.BlazorClient.Models
{
    public class Station
    {
        public int Number { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Status { get; set; } = "";
        public int BikeStands { get; set; }
        public int AvailableBikes { get; set; }
        public int AvailableBikeStands { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
