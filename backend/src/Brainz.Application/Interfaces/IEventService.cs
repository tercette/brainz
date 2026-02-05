using Brainz.Application.DTOs;

namespace Brainz.Application.Interfaces;

public interface IEventService
{
    Task<PagedResultDto<EventDto>> GetUserEventsAsync(
        Guid userId, int page, int pageSize, DateTime? from, DateTime? to);
}
