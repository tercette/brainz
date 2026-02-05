using Brainz.Domain.Entities;
using Brainz.Domain.Interfaces;
using Brainz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brainz.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly BrainzDbContext _context;

    public EventRepository(BrainzDbContext context) => _context = context;

    public async Task<Event?> GetByIdAsync(Guid id)
        => await _context.Events.FindAsync(id);

    public async Task<(IEnumerable<Event> Items, int TotalCount)> GetByUserIdPagedAsync(
        Guid userId, int page, int pageSize, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.Events.Where(e => e.UserId == userId);

        if (from.HasValue)
            query = query.Where(e => e.StartDateTime >= from.Value);
        if (to.HasValue)
            query = query.Where(e => e.EndDateTime <= to.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.StartDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Event?> GetByMicrosoftEventIdAsync(string microsoftEventId)
        => await _context.Events.FirstOrDefaultAsync(e => e.MicrosoftEventId == microsoftEventId);

    public async Task AddRangeAsync(IEnumerable<Event> events)
        => await _context.Events.AddRangeAsync(events);

    public void RemoveRange(IEnumerable<Event> events)
        => _context.Events.RemoveRange(events);

    public async Task<IEnumerable<Event>> GetByUserIdAsync(Guid userId)
        => await _context.Events.Where(e => e.UserId == userId).ToListAsync();
}
