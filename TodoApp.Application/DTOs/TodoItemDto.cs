using TodoApp.Domain.Enums;

namespace TodoApp.Application.DTOs;

public class TodoItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public TodoPriority Priority { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryColor { get; set; }
    public string? Tags { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTodoDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public DateTime? DueDate { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Tags { get; set; }
}

public class UpdateTodoDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Tags { get; set; }
}