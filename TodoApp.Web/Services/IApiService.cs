using TodoApp.Domain.Enums;
using TodoApp.Web.Models;

namespace TodoApp.Web.Services
{
    public interface IApiService
    {
        // Auth
        Task<AuthResponse?> LoginAsync(LoginModel model);
        Task<AuthResponse?> RegisterAsync(RegisterModel model);

        // Todos
        Task<PaginatedResult<TodoItemModel>?> GetTodosAsync(
            TodoStatus? status = null, TodoPriority? priority = null,
            Guid? categoryId = null, string? search = null,
            int page = 1, int pageSize = 20);
        Task<TodoItemModel?> GetTodoByIdAsync(Guid id);
        Task<TodoItemModel?> CreateTodoAsync(CreateTodoModel model);
        Task<TodoItemModel?> UpdateTodoAsync(Guid id, UpdateTodoModel model);
        Task<bool> ChangeTodoStatusAsync(Guid id, TodoStatus newStatus);
        Task<bool> DeleteTodoAsync(Guid id);

        // Categories
        Task<List<CategoryModel>?> GetCategoriesAsync();
        Task<CategoryModel?> CreateCategoryAsync(string name, string color, string? icon);
    }
}
