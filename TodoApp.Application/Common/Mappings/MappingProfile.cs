using AutoMapper;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // TodoItem → TodoItemDto
            CreateMap<TodoItem, TodoItemDto>()
                .ForMember(d => d.StatusLabel,
                    o => o.MapFrom(s => GetStatusLabel(s.Status)))
                .ForMember(d => d.PriorityLabel,
                    o => o.MapFrom(s => GetPriorityLabel(s.Priority)))
                .ForMember(d => d.CategoryName,
                    o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
                .ForMember(d => d.CategoryColor,
                    o => o.MapFrom(s => s.Category != null ? s.Category.Color : null))
                .ForMember(d => d.IsOverdue,
                    o => o.MapFrom(s => s.DueDate.HasValue
                        && s.DueDate.Value < DateTime.UtcNow
                        && s.Status != TodoStatus.Completed
                        && s.Status != TodoStatus.Cancelled));

            // TodoCategory → TodoCategoryDto
            CreateMap<TodoCategory, TodoCategoryDto>()
                .ForMember(d => d.TodoCount,
                    o => o.MapFrom(s => s.TodoItems.Count(t => !t.IsDeleted)));

            // User → UserDto
            CreateMap<User, UserDto>();
        }

        private static string GetStatusLabel(TodoStatus status) => status switch
        {
            TodoStatus.Pending => "Pending",
            TodoStatus.InProgress => "In Progress",
            TodoStatus.Completed => "Completed",
            TodoStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };

        private static string GetPriorityLabel(TodoPriority priority) => priority switch
        {
            TodoPriority.Low => "Low",
            TodoPriority.Medium => "Medium",
            TodoPriority.High => "High",
            TodoPriority.Critical => "Critical",
            _ => "Unknown"
        };
    }
}
