using System.Net.Http.Json;
using DublinBikes.BlazorClient.Models;
using Microsoft.AspNetCore.Components;



namespace DublinBikes.Blazor.Services;

public class StationsApiClient
{
    private readonly HttpClient _http;

    public StationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Station>> GetStationsAsync()
        => await _http.GetFromJsonAsync<List<Station>>("api/stations")
           ?? new();

    public async Task<Station?> GetStationAsync(int number)
        => await _http.GetFromJsonAsync<Station>($"api/stations/{number}");

    public async Task CreateStationAsync(Station station)
        => await _http.PostAsJsonAsync("api/stations", station);
    public Task<HttpResponseMessage> CreateStationAsync(StationDto stationDto)
       => _http.PostAsJsonAsync("api/stations", stationDto);



    public async Task UpdateStationAsync(int number, Station station)
        => await _http.PutAsJsonAsync($"api/stations/{number}", station);
}
