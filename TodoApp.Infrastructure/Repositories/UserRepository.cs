using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null,
            CancellationToken ct = default)
        {
            var query = _dbSet.Where(u => u.Email == email.ToLower());
            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);
            return !await query.AnyAsync(ct);
        }
    }
}
