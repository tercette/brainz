using Brainz.Application.DTOs;
using Brainz.Domain.Enums;

namespace Brainz.Application.Interfaces;

public interface ISyncService
{
    Task SyncUsersAsync();
    Task SyncEventsAsync();
    Task<IEnumerable<SyncLogDto>> GetRecentSyncLogsAsync();
    Task TriggerManualSyncAsync(SyncType syncType);
}
