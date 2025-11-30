using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fs_2025_a_api_demo_002.Data
{
    public class RandomBikeUpdater : BackgroundService
    {
        private readonly JsonBikeRepository _jsonRepo;
        private readonly ILogger<RandomBikeUpdater> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public RandomBikeUpdater(JsonBikeRepository jsonRepo, ILogger<RandomBikeUpdater> logger)
        {
            _jsonRepo = jsonRepo;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RandomBikeUpdater started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _jsonRepo.UpdateRandomAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating bikes");
                }

                await Task.Delay(_interval, stoppingToken);
            }
            _logger.LogInformation("RandomBikeUpdater stopping");
        }
    }
}

