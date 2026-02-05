using Brainz.Domain.Entities;

namespace Brainz.Domain.Interfaces;

public interface ISyncLogRepository
{
    Task<SyncLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<SyncLog>> GetRecentAsync(int count = 20);
    Task AddAsync(SyncLog syncLog);
    void Update(SyncLog syncLog);
}
