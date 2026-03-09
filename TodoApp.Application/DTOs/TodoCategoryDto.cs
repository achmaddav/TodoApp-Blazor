namespace TodoApp.Application.DTOs;

// LAMA - record dengan constructor berparameter, AutoMapper tidak bisa mapping
// public record TodoCategoryDto(Guid Id, string Name, ...);

// BARU - class biasa dengan properties
public class TodoCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#6366F1";
    public string? Icon { get; set; }
    public int TodoCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#6366F1";
    public string? Icon { get; set; }
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#6366F1";
    public string? Icon { get; set; }
}