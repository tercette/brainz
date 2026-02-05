using Brainz.Domain.Entities;

namespace Brainz.Domain.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Event> Items, int TotalCount)> GetByUserIdPagedAsync(
        Guid userId, int page, int pageSize, DateTime? from = null, DateTime? to = null);
    Task<Event?> GetByMicrosoftEventIdAsync(string microsoftEventId);
    Task AddRangeAsync(IEnumerable<Event> events);
    void RemoveRange(IEnumerable<Event> events);
    Task<IEnumerable<Event>> GetByUserIdAsync(Guid userId);
}
