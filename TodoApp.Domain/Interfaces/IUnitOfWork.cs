using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITodoRepository Todos { get; }
        ICategoryRepository Categories { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
