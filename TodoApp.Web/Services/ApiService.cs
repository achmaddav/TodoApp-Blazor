using Blazored.LocalStorage;
using System.Net.Http.Headers;
using TodoApp.Domain.Enums;
using TodoApp.Web.Models;

namespace TodoApp.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public ApiService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        // ─── Auth ────────────────────────────────────────────────────

        public async Task<AuthResponse?> LoginAsync(LoginModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", model);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", model);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        // ─── Todos ───────────────────────────────────────────────────

        public async Task<PaginatedResult<TodoItemModel>?> GetTodosAsync(
            TodoStatus? status = null, TodoPriority? priority = null,
            Guid? categoryId = null, string? search = null,
            int page = 1, int pageSize = 20)
        {
            await SetAuthHeaderAsync();

            var url = $"api/todos?pageNumber={page}&pageSize={pageSize}";
            if (status.HasValue) url += $"&status={(int)status}";
            if (priority.HasValue) url += $"&priority={(int)priority}";
            if (categoryId.HasValue) url += $"&categoryId={categoryId}";
            if (!string.IsNullOrWhiteSpace(search)) url += $"&search={search}";

            var result = await _http.GetFromJsonAsync<ApiResult<PaginatedResult<TodoItemModel>>>(url);
            return result?.Data;
        }

        public async Task<TodoItemModel?> GetTodoByIdAsync(Guid id)
        {
            await SetAuthHeaderAsync();
            var result = await _http.GetFromJsonAsync<ApiResult<TodoItemModel>>($"api/todos/{id}");
            return result?.Data;
        }

        public async Task<TodoItemModel?> CreateTodoAsync(CreateTodoModel model)
        {
            await SetAuthHeaderAsync();
            var response = await _http.PostAsJsonAsync("api/todos", model);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResult<TodoItemModel>>();
            return result?.Data;
        }

        public async Task<TodoItemModel?> UpdateTodoAsync(Guid id, UpdateTodoModel model)
        {
            await SetAuthHeaderAsync();
            var response = await _http.PutAsJsonAsync($"api/todos/{id}", model);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResult<TodoItemModel>>();
            return result?.Data;
        }

        public async Task<bool> ChangeTodoStatusAsync(Guid id, TodoStatus newStatus)
        {
            await SetAuthHeaderAsync();
            var response = await _http.PatchAsJsonAsync(
                $"api/todos/{id}/status", new { NewStatus = (int)newStatus });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTodoAsync(Guid id)
        {
            await SetAuthHeaderAsync();
            var response = await _http.DeleteAsync($"api/todos/{id}");
            return response.IsSuccessStatusCode;
        }

        // ─── Categories ──────────────────────────────────────────────

        public async Task<List<CategoryModel>?> GetCategoriesAsync()
        {
            await SetAuthHeaderAsync();
            var result = await _http
                .GetFromJsonAsync<ApiResult<List<CategoryModel>>>("api/categories");
            return result?.Data;
        }

        public async Task<CategoryModel?> CreateCategoryAsync(
            string name, string color, string? icon)
        {
            await SetAuthHeaderAsync();
            var response = await _http.PostAsJsonAsync("api/categories",
                new { Name = name, Color = color, Icon = icon });
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ApiResult<CategoryModel>>();
            return result?.Data;
        }

        // ─── Helper ──────────────────────────────────────────────────

        private async Task SetAuthHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
