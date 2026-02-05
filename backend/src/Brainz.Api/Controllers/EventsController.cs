using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brainz.Api.Controllers;

[ApiController]
[Route("api/users/{userId:guid}/events")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService) => _eventService = eventService;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<EventDto>), 200)]
    public async Task<IActionResult> GetUserEvents(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _eventService.GetUserEventsAsync(userId, page, pageSize, from, to);
        return Ok(result);
    }
}
