using Brainz.Domain.Common;
using Brainz.Domain.Enums;

namespace Brainz.Domain.Entities;

public class SyncLog : BaseEntity
{
    public SyncType SyncType { get; set; }
    public SyncStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsFailed { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Details { get; set; }
}
