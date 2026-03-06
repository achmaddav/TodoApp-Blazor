using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Todos.Queries.GetAllTodos
{
    public class GetAllTodosQueryHandler
        : IRequestHandler<GetAllTodosQuery, Result<PaginatedList<TodoItemDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetAllTodosQueryHandler(IUnitOfWork uow, IMapper mapper,
                                        ICurrentUserService currentUser)
        {
            _uow = uow;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<PaginatedList<TodoItemDto>>> Handle(
            GetAllTodosQuery request, CancellationToken ct)
        {
            var todos = await _uow.Todos.GetByUserIdAsync(_currentUser.UserId, ct);

            // Filter
            var query = todos.Where(t => !t.IsDeleted).AsQueryable();

            if (request.Status.HasValue)
                query = query.Where(t => t.Status == request.Status.Value);

            if (request.Priority.HasValue)
                query = query.Where(t => t.Priority == request.Priority.Value);

            if (request.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == request.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
                query = query.Where(t => t.Title.Contains(request.SearchKeyword,
                                    StringComparison.OrdinalIgnoreCase));

            // Order & Paginate
            var ordered = query.OrderBy(t => t.DisplayOrder).ThenByDescending(t => t.CreatedAt);
            var totalCount = ordered.Count();
            var items = ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<TodoItemDto>>(items);
            var paginated = PaginatedList<TodoItemDto>.Create(dtos, totalCount,
                                                              request.PageNumber, request.PageSize);

            return Result<PaginatedList<TodoItemDto>>.Success(paginated);
        }
    }
}
