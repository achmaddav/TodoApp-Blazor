using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class TodoRepository : GenericRepository<TodoItem>, ITodoRepository
    {
        public TodoRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<TodoItem>> GetByUserIdAsync(
            Guid userId, CancellationToken ct = default)
            => await _dbSet
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.DisplayOrder)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<TodoItem>> GetByCategoryAsync(
            Guid categoryId, CancellationToken ct = default)
            => await _dbSet
                .Include(t => t.Category)
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<TodoItem>> GetByStatusAsync(
            Guid userId, TodoStatus status, CancellationToken ct = default)
            => await _dbSet
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Status == status)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<TodoItem>> GetOverdueAsync(
            Guid userId, CancellationToken ct = default)
            => await _dbSet
                .Include(t => t.Category)
                .Where(t => t.UserId == userId
                    && t.DueDate.HasValue
                    && t.DueDate.Value < DateTime.UtcNow
                    && t.Status != TodoStatus.Completed
                    && t.Status != TodoStatus.Cancelled)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<TodoItem>> SearchAsync(
            Guid userId, string keyword, CancellationToken ct = default)
            => await _dbSet
                .Include(t => t.Category)
                .Where(t => t.UserId == userId
                    && (t.Title.Contains(keyword) ||
                        (t.Description != null && t.Description.Contains(keyword))))
                .ToListAsync(ct);

        public async Task<int> GetMaxDisplayOrderAsync(
            Guid userId, CancellationToken ct = default)
        {
            var max = await _dbSet
                .Where(t => t.UserId == userId)
                .MaxAsync(t => (int?)t.DisplayOrder, ct);
            return max ?? 0;
        }
    }
}
