using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Categories.Commands.CreateCategory;
using TodoApp.Application.Features.Categories.Commands.DeleteCategory;
using TodoApp.Application.Features.Categories.Commands.UpdateCategory;
using TodoApp.Application.Features.Categories.Queries.GetAllCategories;

namespace TodoApp.API.Controllers;

public class CategoriesController : BaseController
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
        => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery(), ct);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var command = new CreateCategoryCommand(
            dto.Name, dto.Description, dto.Color, dto.Icon);
        var result = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            id, dto.Name, dto.Description, dto.Color, dto.Icon);
        var result = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id), ct);
        return HandleResult(result);
    }
}