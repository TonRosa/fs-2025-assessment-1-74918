using DublinBikes.BlazorClient.Models;
using System.Net.Http.Json;

namespace DublinBikes.BlazorClient.Services
{
    public class StationsApiClient
    {
        private readonly HttpClient _http;

        public StationsApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Station>> GetStationsAsync(
            string? search,
            string? status,
            string? sort,
            int page,
            int pageSize)
        {
            var url =
                $"api/stations?search={search}&status={status}" +
                $"&sort={sort}&page={page}&pageSize={pageSize}";

            return await _http.GetFromJsonAsync<List<Station>>(url) ?? new();
        }

        public async Task<Station?> GetStationAsync(int number)
            => await _http.GetFromJsonAsync<Station>(
                $"api/stations/{number}");

        public async Task CreateStationAsync(Station station)
            => await _http.PostAsJsonAsync("api/stations", station);

        public async Task UpdateStationAsync(int number, Station station)
            => await _http.PutAsJsonAsync($"api/stations/{number}", station);
    }
}
