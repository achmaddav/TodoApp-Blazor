using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Todos.Commands.UpdateTodo
{
    public record UpdateTodoCommand(
        Guid Id,
        string Title,
        string? Description,
        TodoPriority Priority,
        DateTime? DueDate,
        Guid? CategoryId,
        string? Tags
    ) : IRequest<Result<TodoItemDto>>;
}
