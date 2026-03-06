using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<TodoCategory>
    {
        Task<IReadOnlyList<TodoCategory>> GetAllWithCountAsync(CancellationToken ct = default);
        Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null,
                                      CancellationToken ct = default);
    }
}
