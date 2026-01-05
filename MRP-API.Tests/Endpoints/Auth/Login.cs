using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;
using Xunit;


namespace MRP_API.Tests.Endpoints.Auth;

public class LoginTests
{
    private readonly string _baseUrl = "http://localhost:8000";
    private HttpClient _client = new HttpClient();

    [Fact]
    public async Task HandleLogin_Success()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var loginData = new
        {
            username = "admin",
            password = "admin123"
        };

        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/login", content);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HandleLogin_Failure()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var loginData = new
        {
            username = "wronguser",
            password = "wrongpassword"
        };

        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/login", content);

        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task HandleLogin_EmptyCredentials()
    {
        _client.BaseAddress = new Uri(_baseUrl);

        var loginData = new
        {
            username = "",
            password = ""
        };

        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync("/api/auth/login", content);
       
        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }    
}