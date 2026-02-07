namespace Brainz.Application.DTOs;

public record SyncLogDto(
    Guid Id,
    string SyncType,
    string Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int RecordsProcessed,
    int RecordsFailed,
    string? ErrorMessage,
    string? Details
);
