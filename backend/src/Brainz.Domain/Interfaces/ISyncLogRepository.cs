using Brainz.Domain.Entities;
using Brainz.Domain.Enums;

namespace Brainz.Domain.Interfaces;

public interface ISyncLogRepository
{
    Task<SyncLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<SyncLog>> GetRecentAsync(int count = 20);
    Task<bool> IsRunningAsync(SyncType syncType);
    Task MarkStaleAsFailedAsync();
    Task AddAsync(SyncLog syncLog);
    void Update(SyncLog syncLog);
}
