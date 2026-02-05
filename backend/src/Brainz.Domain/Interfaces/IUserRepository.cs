using Brainz.Domain.Entities;

namespace Brainz.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByMicrosoftIdAsync(string microsoftId);
    Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    void Update(User user);
    Task AddRangeAsync(IEnumerable<User> users);
    void UpdateRange(IEnumerable<User> users);
}
