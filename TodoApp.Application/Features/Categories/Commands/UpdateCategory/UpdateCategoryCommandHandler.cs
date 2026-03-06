using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler
        : IRequestHandler<UpdateCategoryCommand, Result<TodoCategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper)
        { _uow = uow; _mapper = mapper; }

        public async Task<Result<TodoCategoryDto>> Handle(
            UpdateCategoryCommand request, CancellationToken ct)
        {
            var category = await _uow.Categories.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(nameof(TodoCategory), request.Id);

            var isUnique = await _uow.Categories
                .IsNameUniqueAsync(request.Name, request.Id, ct);
            if (!isUnique)
                return Result<TodoCategoryDto>.Failure("Category name already exists.");

            category.Update(request.Name, request.Description, request.Color, request.Icon);
            _uow.Categories.Update(category);
            await _uow.SaveChangesAsync(ct);

            return Result<TodoCategoryDto>.Success(_mapper.Map<TodoCategoryDto>(category));
        }
    }
}
