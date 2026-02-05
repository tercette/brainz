using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Brainz.Application.Services;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(ITokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var adminUser = _configuration["Auth:AdminUsername"] ?? "admin";
        var adminPass = _configuration["Auth:AdminPassword"] ?? "YOUR_ADMIN_PASSWORD";

        if (request.Username != adminUser || request.Password != adminPass)
            return Task.FromResult<LoginResponseDto?>(null);

        var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "8");
        var token = _tokenService.GenerateToken(request.Username, "Admin");
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

        return Task.FromResult<LoginResponseDto?>(
            new LoginResponseDto(token, expiresAt, request.Username));
    }
}
