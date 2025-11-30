using fs_2025_a_api_demo_002.Services;


namespace fs_2025_a_api_demo_002.BackgroundServices
{
    public class BikeStationUpdater : BackgroundService
    {
        private readonly JsonDataService _jsonService;
        private readonly ILogger<BikeStationUpdater> _logger;

        public BikeStationUpdater(JsonDataService service, ILogger<BikeStationUpdater> logger)
        {
            _jsonService = service;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rand = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                var stations = await _jsonService.GetAllStationsAsync();

                foreach (var station in stations)
                {
                    int newBikes = rand.Next(0, station.BikeStands + 1);
                    station.AvailableBikes = newBikes;
                    station.AvailableBikeStands = station.BikeStands - newBikes;
                }

                _logger.LogInformation("Stations updated");

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }

}
