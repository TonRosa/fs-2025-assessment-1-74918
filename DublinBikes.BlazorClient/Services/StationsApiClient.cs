using System.Net.Http.Json;
using DublinBikes.BlazorClient.Models;

namespace DublinBikes.Blazor.Services;

public class StationsApiClient
{
    private readonly HttpClient _http;

    public StationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Station>> GetStationsAsync(
        string? search = null,
        string? status = null,
        int page = 1,
        int pageSize = 20)
    {
        var url = $"api/V1/stations?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrWhiteSpace(status))
            url += $"&status={Uri.EscapeDataString(status)}";

        return await _http.GetFromJsonAsync<List<Station>>(url) ?? new();
    }

    public async Task<Station?> GetStationAsync(int number)
        => await _http.GetFromJsonAsync<Station>($"api/V1/stations/Station/Number{number}");

    public async Task<Station?> CreateStationAsync(Station station)
    {
        var response = await _http.PostAsJsonAsync("api/V1/stations", station);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Station>();
    }

    public async Task UpdateStationAsync(int number, Station station)
    {
        var response = await _http.PutAsJsonAsync($"api/V1/stations/{number}", station);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStationAsync(int number)
    {
        var response = await _http.DeleteAsync($"api/V1/stations/{number}");
        response.EnsureSuccessStatusCode();
    }
}