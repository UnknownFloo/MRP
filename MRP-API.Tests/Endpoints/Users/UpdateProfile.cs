using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Xunit;

namespace MRP_API.Tests.Endpoints.Users;

public class UpdateProfileTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private readonly HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleUpdateProfile_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new
        {
            new_description = "Updated bio information"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/users/profile", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateProfile_Unauthorized()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var updateData = new
        {
            email = "updated@example.com"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/users/profile", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateProfile_InvalidForm()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new
        {
            description = "1231231233121"
        };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/users/profile", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task HandleUpdateProfile_EmptyData()
    {
        _client.BaseAddress = new Uri(_baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "admin-mrpToken");

        var updateData = new { };

        var json = JsonSerializer.Serialize(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/users/profile", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}