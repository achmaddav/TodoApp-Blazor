using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Todos.Commands.ChangeTodoStatus
{
    public class ChangeTodoStatusCommandHandler
        : IRequestHandler<ChangeTodoStatusCommand, Result<TodoItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ChangeTodoStatusCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<TodoItemDto>> Handle(ChangeTodoStatusCommand request,
                                                       CancellationToken ct)
        {
            var todo = await _uow.Todos.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(nameof(TodoItem), request.Id);

            switch (request.NewStatus)
            {
                case TodoStatus.InProgress: todo.MarkAsInProgress(); break;
                case TodoStatus.Completed: todo.MarkAsCompleted(); break;
                case TodoStatus.Cancelled: todo.Cancel(); break;
                case TodoStatus.Pending: todo.Reopen(); break;
            }

            _uow.Todos.Update(todo);
            await _uow.SaveChangesAsync(ct);

            return Result<TodoItemDto>.Success(_mapper.Map<TodoItemDto>(todo));
        }
    }
}
