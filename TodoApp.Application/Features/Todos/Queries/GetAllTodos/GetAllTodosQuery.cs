using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Todos.Queries.GetAllTodos
{
    public record GetAllTodosQuery(
        TodoStatus? Status = null,
        TodoPriority? Priority = null,
        Guid? CategoryId = null,
        string? SearchKeyword = null,
        int PageNumber = 1,
        int PageSize = 20
    ) : IRequest<Result<PaginatedList<TodoItemDto>>>;
}
