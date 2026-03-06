using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Todos.Commands.UpdateTodo
{
    public class UpdateTodoCommandHandler
        : IRequestHandler<UpdateTodoCommand, Result<TodoItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateTodoCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<TodoItemDto>> Handle(UpdateTodoCommand request,
                                                       CancellationToken ct)
        {
            var todo = await _uow.Todos.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(nameof(TodoItem), request.Id);

            todo.UpdateDetails(request.Title, request.Description,
                               request.Priority, request.DueDate,
                               request.CategoryId, request.Tags);

            _uow.Todos.Update(todo);
            await _uow.SaveChangesAsync(ct);

            return Result<TodoItemDto>.Success(_mapper.Map<TodoItemDto>(todo));
        }
    }
}
