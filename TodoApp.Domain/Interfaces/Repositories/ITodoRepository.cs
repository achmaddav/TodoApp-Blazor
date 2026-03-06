using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface ITodoRepository : IGenericRepository<TodoItem>
    {
        Task<IReadOnlyList<TodoItem>> GetByUserIdAsync(Guid userId,
                                                        CancellationToken ct = default);
        Task<IReadOnlyList<TodoItem>> GetByCategoryAsync(Guid categoryId,
                                                          CancellationToken ct = default);
        Task<IReadOnlyList<TodoItem>> GetByStatusAsync(Guid userId, TodoStatus status,
                                                        CancellationToken ct = default);
        Task<IReadOnlyList<TodoItem>> GetOverdueAsync(Guid userId,
                                                       CancellationToken ct = default);
        Task<IReadOnlyList<TodoItem>> SearchAsync(Guid userId, string keyword,
                                                  CancellationToken ct = default);
        Task<int> GetMaxDisplayOrderAsync(Guid userId, CancellationToken ct = default);
    }
}
