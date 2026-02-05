using Brainz.Domain.Entities;
using Brainz.Domain.Interfaces;
using Brainz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brainz.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BrainzDbContext _context;

    public UserRepository(BrainzDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FindAsync(id);

    public async Task<User?> GetByMicrosoftIdAsync(string microsoftId)
        => await _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == microsoftId);

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(u =>
                u.DisplayName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term) ||
                (u.Department != null && u.Department.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.DisplayName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users.ToListAsync();

    public async Task AddAsync(User user)
        => await _context.Users.AddAsync(user);

    public void Update(User user)
        => _context.Users.Update(user);

    public async Task AddRangeAsync(IEnumerable<User> users)
        => await _context.Users.AddRangeAsync(users);

    public void UpdateRange(IEnumerable<User> users)
        => _context.Users.UpdateRange(users);
}
