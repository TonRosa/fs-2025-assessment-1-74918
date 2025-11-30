using fs_2025_a_api_demo_002.Data;
using Microsoft.Azure.Cosmos;

namespace fs_2025_a_api_demo_002.Startup
{
    public static class DependenciesConfig
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
          
            builder.Services.AddTransient<BikeData>();
        
            builder.Services.AddMemoryCache();

            // Json-backed repository (V1)
            builder.Services.AddSingleton<JsonBikeRepository>();
            builder.Services.AddSingleton<IBikeRepository>(sp => sp.GetRequiredService<JsonBikeRepository>());

            // Cosmos client + repo (V2) - requires appsettings Cosmos:Endpoint / Key / Database / Container
            var endpoint = builder.Configuration["Cosmos:Endpoint"];
            var key = builder.Configuration["Cosmos:Key"];
            if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(key))
            {
                var client = new CosmosClient(endpoint, key);
                builder.Services.AddSingleton(client);
                builder.Services.AddSingleton<CosmosBikeRepository>();
            }

            // Background updater that mutates the JSON dataset periodically
            builder.Services.AddHostedService<RandomBikeUpdater>();
        }
    }
}
