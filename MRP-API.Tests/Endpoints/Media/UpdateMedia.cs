using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Media;

public class UpdateMediaTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();
    private readonly int _mediaId = 1;

    [Fact]
    public async Task HandleUpdateMedia_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new
        {
            title = "Updated Test Movie",
            description = "An updated test movie for unit testing",
            mediaType = "Movie",
            releaseYear = 2025,
            genre = new[] { "Action", "Sci-Fi" },
            ageRestriction = 18
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/media?mediaID={_mediaId}", content);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HandleUpdateMedia_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var updateData = new
        {
            title = "Updated Test Movie",
            description = "An updated test movie for unit testing"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/media?mediaID={_mediaId}", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateMedia_MediaNotFound()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

         var updateData = new
        {
            title = "Updated Test Movie",
            description = "An updated test movie for unit testing",
            mediaType = "Movie",
            releaseYear = 2025,
            genre = new[] { "Action", "Sci-Fi" },
            ageRestriction = 18
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/media?mediaID=9999", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateMedia_MediaNotOwned()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "debugUser-mrpToken");

         var updateData = new
        {
            title = "Updated Test Movie",
            description = "An updated test movie for unit testing",
            mediaType = "Movie",
            releaseYear = 2025,
            genre = new[] { "Action", "Sci-Fi" },
            ageRestriction = 18
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/media?mediaID=1", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateMedia_MissingMediaID()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new
        {
            title = "Updated Test Movie"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/media", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}