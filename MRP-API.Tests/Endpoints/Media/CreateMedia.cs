using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Media;

public class CreateMediaTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleCreateMedia_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var mediaData = new
        {
            title = "Test Movie",
            description = "A test movie for unit testing",
            mediaType = "Movie",
            releaseYear = 2024,
            genre = new[] { "Action", "Drama" },
            ageRestriction = 16
        };

        var json = JsonSerializer.Serialize(mediaData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/media", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleCreateMedia_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var mediaData = new
        {
            title = "Test Movie",
            description = "A test movie for unit testing",
            mediaType = "Movie",
            releaseYear = 2024,
            genre = new[] { "Action", "Drama" },
            ageRestriction = 16
        };

        var json = JsonSerializer.Serialize(mediaData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/media", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleCreateMedia_MissingTitle()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var mediaData = new
        {
            description = "A test movie for unit testing",
            mediaType = "Movie",
            releaseYear = 2024,
            genre = new[] { "Action", "Drama" },
            ageRestriction = 16
        };

        var json = JsonSerializer.Serialize(mediaData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/media", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleCreateMedia_MissingDescription()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var mediaData = new
        {
            title = "Test Movie",
            mediaType = "Movie",
            releaseYear = 2024,
            genre = new[] { "Action", "Drama" },
            ageRestriction = 16
        };

        var json = JsonSerializer.Serialize(mediaData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/media", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}