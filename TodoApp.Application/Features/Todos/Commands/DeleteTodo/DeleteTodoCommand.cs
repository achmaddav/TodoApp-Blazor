using MediatR;
using TodoApp.Application.Common.Models;

namespace TodoApp.Application.Features.Todos.Commands.DeleteTodo
{
    public record DeleteTodoCommand(Guid Id) : IRequest<Result>;
}
