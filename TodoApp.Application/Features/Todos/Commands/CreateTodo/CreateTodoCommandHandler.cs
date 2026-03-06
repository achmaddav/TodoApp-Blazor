using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Todos.Commands.CreateTodo
{
    public class CreateTodoCommandHandler
        : IRequestHandler<CreateTodoCommand, Result<TodoItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public CreateTodoCommandHandler(IUnitOfWork uow, IMapper mapper,
                                         ICurrentUserService currentUser)
        {
            _uow = uow;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<TodoItemDto>> Handle(CreateTodoCommand request,
                                                       CancellationToken ct)
        {
            var maxOrder = await _uow.Todos.GetMaxDisplayOrderAsync(_currentUser.UserId, ct);

            var todo = new TodoItem(
                title: request.Title,
                userId: _currentUser.UserId,
                description: request.Description,
                priority: request.Priority,
                dueDate: request.DueDate,
                categoryId: request.CategoryId
            );

            todo.SetDisplayOrder(maxOrder + 1);

            await _uow.Todos.AddAsync(todo, ct);
            await _uow.SaveChangesAsync(ct);

            // Reload with category
            var created = await _uow.Todos.GetByIdAsync(todo.Id, ct);
            var dto = _mapper.Map<TodoItemDto>(created);

            return Result<TodoItemDto>.Created(dto, "Todo created successfully.");
        }
    }
}
