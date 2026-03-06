using System.Linq.Expressions;
using TodoApp.Domain.Common;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        // Query
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                         CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
                                      CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate,
                                CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null,
                              CancellationToken ct = default);

        // Command
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
