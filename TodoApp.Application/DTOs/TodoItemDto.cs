using TodoApp.Domain.Enums;

namespace TodoApp.Application.DTOs
{
    public record TodoItemDto(
        Guid Id,
        string Title,
        string? Description,
        TodoStatus Status,
        string StatusLabel,
        TodoPriority Priority,
        string PriorityLabel,
        DateTime? DueDate,
        bool IsOverdue,
        DateTime? CompletedAt,
        Guid? CategoryId,
        string? CategoryName,
        string? CategoryColor,
        string? Tags,
        int DisplayOrder,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );

    public record CreateTodoDto(
        string Title,
        string? Description,
        TodoPriority Priority,
        DateTime? DueDate,
        Guid? CategoryId,
        string? Tags
    );

    public record UpdateTodoDto(
        string Title,
        string? Description,
        TodoPriority Priority,
        DateTime? DueDate,
        Guid? CategoryId,
        string? Tags
    );
}
