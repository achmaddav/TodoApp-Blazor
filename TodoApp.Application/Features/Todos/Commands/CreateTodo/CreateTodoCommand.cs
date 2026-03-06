using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Todos.Commands.CreateTodo
{
    public record CreateTodoCommand(
        string Title,
        string? Description,
        TodoPriority Priority,
        DateTime? DueDate,
        Guid? CategoryId,
        string? Tags
    ) : IRequest<Result<TodoItemDto>>;
}
