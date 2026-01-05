using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;
using Xunit;

namespace MRP_API.Tests.Endpoints.Auth;

public class RegisterTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleRegister_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var registerData = new
        {
            username = "newuser",
            password = "newpassword"
        };

        var json = JsonSerializer.Serialize(registerData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/register", content);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HandleRegister_Failure()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var registerData = new
        {
            username = "admin",
            password = "admin123"
        };

        var json = JsonSerializer.Serialize(registerData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/register", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task HandleRegister_EmptyCredentials()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var registerData = new
        {
            username = "",
            password = ""
        };

        var json = JsonSerializer.Serialize(registerData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/register", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}