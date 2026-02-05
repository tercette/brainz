using Brainz.Domain.Common;

namespace Brainz.Domain.Entities;

public class User : BaseEntity
{
    public string MicrosoftId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastSyncedAt { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
