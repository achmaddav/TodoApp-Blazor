using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Todos.Commands.DeleteTodo
{
    public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public DeleteTodoCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken ct)
        {
            var todo = await _uow.Todos.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(nameof(TodoItem), request.Id);

            todo.SoftDelete(_currentUser.Email);
            _uow.Todos.Update(todo);
            await _uow.SaveChangesAsync(ct);

            return Result.Success("Todo deleted successfully.");
        }
    }
}
