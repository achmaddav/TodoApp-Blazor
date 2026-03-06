using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Todos.Commands.ChangeTodoStatus;
using TodoApp.Application.Features.Todos.Commands.CreateTodo;
using TodoApp.Application.Features.Todos.Commands.DeleteTodo;
using TodoApp.Application.Features.Todos.Commands.UpdateTodo;
using TodoApp.Application.Features.Todos.Queries.GetAllTodos;
using TodoApp.Application.Features.Todos.Queries.GetTodoById;
using TodoApp.Domain.Enums;

namespace TodoApp.API.Controllers
{
    public class TodosController : BaseController
    {
        private readonly IMediator _mediator;

        public TodosController(IMediator mediator)
            => _mediator = mediator;

        /// <summary>Get all todos with optional filters</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] TodoStatus? status,
            [FromQuery] TodoPriority? priority,
            [FromQuery] Guid? categoryId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var query = new GetAllTodosQuery(status, priority, categoryId,
                                             search, pageNumber, pageSize);
            var result = await _mediator.Send(query, ct);
            return HandleResult(result);
        }

        /// <summary>Get todo by ID</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTodoByIdQuery(id), ct);
            return HandleResult(result);
        }

        /// <summary>Create new todo</summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTodoDto dto, CancellationToken ct)
        {
            var command = new CreateTodoCommand(
                dto.Title, dto.Description, dto.Priority,
                dto.DueDate, dto.CategoryId, dto.Tags);
            var result = await _mediator.Send(command, ct);
            return HandleResult(result);
        }

        /// <summary>Update existing todo</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id, [FromBody] UpdateTodoDto dto, CancellationToken ct)
        {
            var command = new UpdateTodoCommand(
                id, dto.Title, dto.Description, dto.Priority,
                dto.DueDate, dto.CategoryId, dto.Tags);
            var result = await _mediator.Send(command, ct);
            return HandleResult(result);
        }

        /// <summary>Change todo status</summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(
            Guid id, [FromBody] ChangeTodoStatusRequest request, CancellationToken ct)
        {
            var command = new ChangeTodoStatusCommand(id, request.NewStatus);
            var result = await _mediator.Send(command, ct);
            return HandleResult(result);
        }

        /// <summary>Delete todo (soft delete)</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteTodoCommand(id), ct);
            return HandleResult(result);
        }
    }

    // Request model untuk PATCH
    public record ChangeTodoStatusRequest(TodoStatus NewStatus);
}
