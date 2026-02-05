namespace Brainz.Application.DTOs;

public record UserDto(
    Guid Id,
    string DisplayName,
    string Email,
    string? GivenName,
    string? Surname,
    string? Department,
    string? JobTitle,
    bool IsActive,
    DateTime? LastSyncedAt
);
