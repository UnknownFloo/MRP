using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Media;

public class SearchMediaTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleSearchMedia_ByTitle_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.GetAsync("/api/media?title=Test Movie");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleSearchMedia_ByGenre_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var response = await _client.GetAsync("/api/media?genre=Action");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleSearchMedia_ByReleaseYear_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");
        
        var response = await _client.GetAsync("/api/media?release_Year=2024");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleSearchMedia_MultipleFilters_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");
     


        var response = await _client.GetAsync("/api/media?title=Test&genre=Action&releaseYear=2024");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task HandleSearchMedia_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var response = await _client.GetAsync("/api/media?title=Test Movie");

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}