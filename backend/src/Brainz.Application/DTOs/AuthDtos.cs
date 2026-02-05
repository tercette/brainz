namespace Brainz.Application.DTOs;

public record LoginRequestDto(string Username, string Password);

public record LoginResponseDto(string Token, DateTime ExpiresAt, string Username);
