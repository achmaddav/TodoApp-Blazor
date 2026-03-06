using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler
        : IRequestHandler<CreateCategoryCommand, Result<TodoCategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper)
        { _uow = uow; _mapper = mapper; }

        public async Task<Result<TodoCategoryDto>> Handle(
            CreateCategoryCommand request, CancellationToken ct)
        {
            var isUnique = await _uow.Categories.IsNameUniqueAsync(request.Name, null, ct);
            if (!isUnique)
                return Result<TodoCategoryDto>.Failure("Category name already exists.");

            var category = new TodoCategory(
                request.Name, request.Description, request.Color, request.Icon);

            await _uow.Categories.AddAsync(category, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<TodoCategoryDto>.Created(_mapper.Map<TodoCategoryDto>(category));
        }
    }
}
