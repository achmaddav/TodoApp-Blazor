using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler
        : IRequestHandler<GetAllCategoriesQuery, Result<List<TodoCategoryDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper)
        { _uow = uow; _mapper = mapper; }

        public async Task<Result<List<TodoCategoryDto>>> Handle(
    GetAllCategoriesQuery request, CancellationToken ct)
        {
            try
            {
                var categories = await _uow.Categories.GetAllWithCountAsync(ct);
                var dtos = _mapper.Map<List<TodoCategoryDto>>(categories);
                return Result<List<TodoCategoryDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                // Tambahkan ini untuk debug
                Console.WriteLine($"GetAllCategories Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
