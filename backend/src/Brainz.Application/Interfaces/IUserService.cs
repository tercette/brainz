using Brainz.Application.DTOs;

namespace Brainz.Application.Interfaces;

public interface IUserService
{
    Task<PagedResultDto<UserDto>> GetUsersAsync(int page, int pageSize, string? searchTerm);
    Task<UserDto?> GetUserByIdAsync(Guid id);
}
