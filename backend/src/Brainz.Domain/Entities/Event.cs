using Brainz.Domain.Common;

namespace Brainz.Domain.Entities;

public class Event : BaseEntity
{
    public string MicrosoftEventId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? BodyPreview { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? Location { get; set; }
    public bool IsAllDay { get; set; }
    public string? OrganizerName { get; set; }
    public string? OrganizerEmail { get; set; }
    public bool IsCancelled { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
