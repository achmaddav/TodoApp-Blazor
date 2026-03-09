using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null,
            CancellationToken ct = default);
        //Task UpdateAsync(User user, CancellationToken ct = default);
    }
}
