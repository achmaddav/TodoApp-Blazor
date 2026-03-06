using Microsoft.EntityFrameworkCore.Storage;
using TodoApp.Domain.Interfaces;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Repositories;

namespace TodoApp.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy-initialized repositories
        private ITodoRepository? _todos;
        private ICategoryRepository? _categories;
        private IUserRepository? _users;

        public UnitOfWork(AppDbContext context) => _context = context;

        public ITodoRepository Todos
            => _todos ??= new TodoRepository(_context);

        public ICategoryRepository Categories
            => _categories ??= new CategoryRepository(_context);

        public IUserRepository Users 
            => _users ??= new UserRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
            => _transaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction.");
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return;
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
