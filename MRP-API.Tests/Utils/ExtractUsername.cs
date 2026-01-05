namespace MRP_API.Tests.Utils;

using MRP.Utils;
using Xunit;

public class ExtractUsernameTests
{
    [Fact]
    public void ExtractUsername()
    {
        string Token = "Bearer admin-mrpToken";
        Assert.Equal("admin",  GetUsername.ExtractUsername(Token));
    }

    [Fact]
    public void ExtractUsername_InvalidToken_ThrowsException()
    {
        string Token = "InvalidTokenFormat";
        Assert.Throws<ArgumentException>(() => GetUsername.ExtractUsername(Token));
    }

    [Fact]
    public void ExtractUsername_EmptyToken_ThrowsException()
    {
        string Token = "";
        Assert.Throws<ArgumentException>(() => GetUsername.ExtractUsername(Token));
    }
}
