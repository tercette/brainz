using Brainz.Domain.Interfaces;
using Brainz.Infrastructure.Data;

namespace Brainz.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BrainzDbContext _context;

    public UnitOfWork(
        BrainzDbContext context,
        IUserRepository users,
        IEventRepository events,
        ISyncLogRepository syncLogs)
    {
        _context = context;
        Users = users;
        Events = events;
        SyncLogs = syncLogs;
    }

    public IUserRepository Users { get; }
    public IEventRepository Events { get; }
    public ISyncLogRepository SyncLogs { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
