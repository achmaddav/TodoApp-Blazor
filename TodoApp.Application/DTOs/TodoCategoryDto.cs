namespace TodoApp.Application.DTOs
{
    public record TodoCategoryDto(
        Guid Id,
        string Name,
        string? Description,
        string Color,
        string? Icon,
        int TodoCount,
        DateTime CreatedAt
    );

    public record CreateCategoryDto(
        string Name,
        string? Description,
        string Color,
        string? Icon
    );

    public record UpdateCategoryDto(
        string Name,
        string? Description,
        string Color,
        string? Icon
    );
}
