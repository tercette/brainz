using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Brainz.Domain.Interfaces;

namespace Brainz.Application.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<EventDto>> GetUserEventsAsync(
        Guid userId, int page, int pageSize, DateTime? from, DateTime? to)
    {
        var (items, totalCount) = await _unitOfWork.Events
            .GetByUserIdPagedAsync(userId, page, pageSize, from, to);

        var dtos = items.Select(e => new EventDto(
            e.Id, e.Subject, e.BodyPreview, e.StartDateTime, e.EndDateTime,
            e.Location, e.IsAllDay, e.OrganizerName, e.IsCancelled));

        return new PagedResultDto<EventDto>(dtos, totalCount, page, pageSize);
    }
}
