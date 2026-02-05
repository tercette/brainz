namespace Brainz.Application.DTOs;

public record EventDto(
    Guid Id,
    string Subject,
    string? BodyPreview,
    DateTime StartDateTime,
    DateTime EndDateTime,
    string? Location,
    bool IsAllDay,
    string? OrganizerName,
    bool IsCancelled
);
