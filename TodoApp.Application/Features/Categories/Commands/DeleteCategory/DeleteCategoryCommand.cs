using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCategoryCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var category = await _uow.Categories.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException(nameof(TodoCategory), request.Id);

            _uow.Categories.Remove(category);
            await _uow.SaveChangesAsync(ct);

            return Result.Success("Category deleted successfully.");
        }
    }
}
