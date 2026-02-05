using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Brainz.Domain.Interfaces;

namespace Brainz.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<UserDto>> GetUsersAsync(int page, int pageSize, string? searchTerm)
    {
        var (items, totalCount) = await _unitOfWork.Users.GetPagedAsync(page, pageSize, searchTerm);

        var dtos = items.Select(u => new UserDto(
            u.Id, u.DisplayName, u.Email, u.GivenName, u.Surname,
            u.Department, u.JobTitle, u.IsActive, u.LastSyncedAt));

        return new PagedResultDto<UserDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return null;

        return new UserDto(
            user.Id, user.DisplayName, user.Email, user.GivenName, user.Surname,
            user.Department, user.JobTitle, user.IsActive, user.LastSyncedAt);
    }
}
