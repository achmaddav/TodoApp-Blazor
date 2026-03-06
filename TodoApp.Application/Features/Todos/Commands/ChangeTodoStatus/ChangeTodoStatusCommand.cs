using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Todos.Commands.ChangeTodoStatus
{
    public record ChangeTodoStatusCommand(
        Guid Id,
        TodoStatus NewStatus
    ) : IRequest<Result<TodoItemDto>>;
}
