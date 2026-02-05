using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Brainz.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Brainz.UnitTests.Application;

public class AuthServiceTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly IConfiguration _configuration;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:AdminUsername"] = "admin",
                ["Auth:AdminPassword"] = "YOUR_ADMIN_PASSWORD",
                ["Jwt:ExpirationHours"] = "8"
            })
            .Build();

        _sut = new AuthService(_tokenServiceMock.Object, _configuration);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        _tokenServiceMock.Setup(t => t.GenerateToken("admin", "Admin"))
            .Returns("fake-jwt-token");

        var result = await _sut.LoginAsync(new LoginRequestDto("admin", "YOUR_ADMIN_PASSWORD"));

        result.Should().NotBeNull();
        result!.Token.Should().Be("fake-jwt-token");
        result.Username.Should().Be("admin");
    }

    [Fact]
    public async Task LoginAsync_InvalidUsername_ReturnsNull()
    {
        var result = await _sut.LoginAsync(new LoginRequestDto("wrong", "YOUR_ADMIN_PASSWORD"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var result = await _sut.LoginAsync(new LoginRequestDto("admin", "wrongpassword"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_SetsExpiresAt()
    {
        _tokenServiceMock.Setup(t => t.GenerateToken("admin", "Admin"))
            .Returns("token");

        var before = DateTime.UtcNow;
        var result = await _sut.LoginAsync(new LoginRequestDto("admin", "YOUR_ADMIN_PASSWORD"));
        var after = DateTime.UtcNow;

        result.Should().NotBeNull();
        result!.ExpiresAt.Should().BeAfter(before.AddHours(7));
        result.ExpiresAt.Should().BeBefore(after.AddHours(9));
    }
}
