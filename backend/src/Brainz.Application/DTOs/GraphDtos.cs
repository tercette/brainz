namespace Brainz.Application.DTOs;

public class GraphUserDto
{
    public string MicrosoftId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
}

public class GraphEventDto
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
}
