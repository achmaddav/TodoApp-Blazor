using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<TodoCategory>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<TodoCategory>> GetAllWithCountAsync(
            CancellationToken ct = default)
            => await _dbSet
                .Include(c => c.TodoItems.Where(t => !t.IsDeleted))
                .OrderBy(c => c.Name)
                .ToListAsync(ct);

        public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null,
            CancellationToken ct = default)
        {
            var query = _dbSet.Where(c => c.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return !await query.AnyAsync(ct);
        }
    }
}
