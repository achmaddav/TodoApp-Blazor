using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Todos.Queries.GetTodoById
{
    public record GetTodoByIdQuery(Guid Id) : IRequest<Result<TodoItemDto>>;

    public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, Result<TodoItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetTodoByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
        { _uow = uow; _mapper = mapper; }

        public async Task<Result<TodoItemDto>> Handle(GetTodoByIdQuery request, CancellationToken ct)
        {
            var todo = await _uow.Todos.GetByIdAsync(request.Id, ct);
            if (todo is null || todo.IsDeleted)
                return Result<TodoItemDto>.NotFound($"Todo '{request.Id}' not found.");

            return Result<TodoItemDto>.Success(_mapper.Map<TodoItemDto>(todo));
        }
    }
}
