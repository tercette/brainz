using Brainz.Application.DTOs;

namespace Brainz.Application.Interfaces;

public interface IMicrosoftGraphService
{
    Task<List<GraphUserDto>> GetAllUsersAsync();
    Task<List<GraphEventDto>> GetUserEventsAsync(string microsoftUserId, DateTime from, DateTime to);
}
