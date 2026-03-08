using Microsoft.AspNetCore.Components;
using MudBlazor;
using TodoApp.Domain.Enums;
using TodoApp.Web.Components.Todos;
using TodoApp.Web.Models;
using TodoApp.Web.Services;

namespace TodoApp.Web.Pages.Todos;

public partial class TodoList
{
    // ─── Inject ──────────────────────────────────────────────────
    [Inject] private IApiService ApiService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    // ─── State ───────────────────────────────────────────────────
    private List<TodoItemModel> _todos = new();
    private bool _isLoading = true;
    private int _totalCount, _totalPages, _currentPage = 1;
    private string _searchText = string.Empty;
    private TodoStatus? _statusFilter;
    private TodoPriority? _priorityFilter;
    private List<(string Label, int Count, string Color)> _stats = new();

    // ─── Lifecycle ───────────────────────────────────────────────
    protected override async Task OnInitializedAsync()
        => await LoadTodosAsync();

    // ─── Data ────────────────────────────────────────────────────
    private async Task LoadTodosAsync()
    {
        _isLoading = true;
        try
        {
            var result = await ApiService.GetTodosAsync(
                _statusFilter, _priorityFilter, null,
                _searchText, _currentPage);

            if (result is not null)
            {
                _todos = result.Items;
                _totalCount = result.TotalCount;
                _totalPages = result.TotalPages;
                UpdateStats();
            }
        }
        finally { _isLoading = false; }
    }

    private void UpdateStats()
    {
        _stats = new()
        {
            ("Total",       _totalCount, "#6366F1"),
            ("Pending",     _todos.Count(t => t.Status == TodoStatus.Pending),    "#F59E0B"),
            ("In Progress", _todos.Count(t => t.Status == TodoStatus.InProgress), "#3B82F6"),
            ("Completed",   _todos.Count(t => t.Status == TodoStatus.Completed),  "#10B981"),
        };
    }

    // ─── Filter & Search ─────────────────────────────────────────
    private async Task OnSearchChanged(string value)
    {
        _searchText = value;
        _currentPage = 1;
        await LoadTodosAsync();
    }

    private async Task OnFilterChanged<T>(T value)
    {
        _currentPage = 1;
        await LoadTodosAsync();
    }

    private async Task ClearFilters()
    {
        _searchText = string.Empty;
        _statusFilter = null;
        _priorityFilter = null;
        _currentPage = 1;
        await LoadTodosAsync();
    }

    private async Task OnPageChanged(int page)
    {
        _currentPage = page;
        await LoadTodosAsync();
    }

    // ─── CRUD Actions ────────────────────────────────────────────
    private async Task OnStatusChanged((Guid Id, TodoStatus Status) args)
    {
        var success = await ApiService.ChangeTodoStatusAsync(args.Id, args.Status);
        if (success)
        {
            Snackbar.Add("Status updated!", Severity.Success);
            await LoadTodosAsync();
        }
        else Snackbar.Add("Failed to update status.", Severity.Error);
    }

    private async Task OpenCreateDialog()
    {
        var categories = await ApiService.GetCategoriesAsync() ?? new();
        var parameters = new DialogParameters
        {
            ["Categories"] = categories
        };
        var dialog = await DialogService.ShowAsync<CreateTodoDialog>(
            "New Todo", parameters,
            new DialogOptions
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseButton = true
            });

        var result = await dialog.Result;
        if (!result.Canceled) await LoadTodosAsync();
    }

    private async Task OpenEditDialog(TodoItemModel todo)
    {
        var categories = await ApiService.GetCategoriesAsync() ?? new();
        var parameters = new DialogParameters
        {
            ["Todo"] = todo,
            ["Categories"] = categories
        };
        var dialog = await DialogService.ShowAsync<EditTodoDialog>(
            "Edit Todo", parameters,
            new DialogOptions
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseButton = true
            });

        var result = await dialog.Result;
        if (!result.Canceled) await LoadTodosAsync();
    }

    private async Task DeleteTodo(Guid id)
    {
        var success = await ApiService.DeleteTodoAsync(id);
        if (success)
        {
            Snackbar.Add("Todo deleted.", Severity.Info);
            await LoadTodosAsync();
        }
        else Snackbar.Add("Failed to delete todo.", Severity.Error);
    }

    private static string GetStatCardStyle(string color) =>
        $"border-radius:12px; border-left:4px solid {color};";

    private static string GetStatTextStyle(string color) =>
        $"color:{color};";
}