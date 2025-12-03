using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


namespace BikeApiTests;

public class StationEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StationEndpointTests(WebApplicationFactory<Program> factory)
    {
        // Boota a API inteira em memória
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStations_ReturnsOk()
    {
        // Arrange ? nada

        // Act ? chama o endpoint real
        var response = await _client.GetAsync("/api/v1/station");

        // Assert ? valida HTTP 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Opcional: lê o JSON para garantir que tem conteúdo
        var content = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(content);
    }
}