using Brainz.Domain.Entities;
using Brainz.Domain.Enums;
using Brainz.Domain.Interfaces;
using Brainz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brainz.Infrastructure.Repositories;

public class SyncLogRepository : ISyncLogRepository
{
    private readonly BrainzDbContext _context;

    public SyncLogRepository(BrainzDbContext context) => _context = context;

    public async Task<SyncLog?> GetByIdAsync(Guid id)
        => await _context.SyncLogs.FindAsync(id);

    public async Task<IEnumerable<SyncLog>> GetRecentAsync(int count = 20)
        => await _context.SyncLogs
            .OrderByDescending(s => s.StartedAt)
            .Take(count)
            .ToListAsync();

    public async Task<bool> IsRunningAsync(SyncType syncType)
        => await _context.SyncLogs
            .AnyAsync(s => s.SyncType == syncType && s.Status == SyncStatus.Running);

    public async Task MarkStaleAsFailedAsync()
    {
        var staleLogs = await _context.SyncLogs
            .Where(s => s.Status == SyncStatus.Running)
            .ToListAsync();

        foreach (var log in staleLogs)
        {
            log.Status = SyncStatus.Failed;
            log.CompletedAt = DateTime.UtcNow;
            log.ErrorMessage = "Processo interrompido por reinicialização";
        }
    }

    public async Task AddAsync(SyncLog syncLog)
        => await _context.SyncLogs.AddAsync(syncLog);

    public void Update(SyncLog syncLog)
        => _context.SyncLogs.Update(syncLog);
}
