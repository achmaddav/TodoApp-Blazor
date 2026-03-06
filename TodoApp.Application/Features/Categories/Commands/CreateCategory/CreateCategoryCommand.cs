using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(
        string Name,
        string? Description,
        string Color,
        string? Icon
    ) : IRequest<Result<TodoCategoryDto>>;
}
