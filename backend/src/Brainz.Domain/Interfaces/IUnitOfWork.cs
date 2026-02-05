namespace Brainz.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IEventRepository Events { get; }
    ISyncLogRepository SyncLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
