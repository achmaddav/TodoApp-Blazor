using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities
{
    public class TodoCategory : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string Color { get; private set; } = "#6366F1"; // default indigo
        public string? Icon { get; private set; }

        // Navigation
        private readonly List<TodoItem> _todoItems = new();
        public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();

        // EF Core constructor
        private TodoCategory() { }

        public TodoCategory(string name, string? description = null,
                            string color = "#6366F1", string? icon = null)
        {
            SetName(name);
            Description = description;
            Color = color;
            Icon = icon;
        }

        // Domain Methods
        public void Update(string name, string? description, string color, string? icon)
        {
            SetName(name);
            Description = description;
            Color = color;
            Icon = icon;
            UpdatedAt = DateTime.UtcNow;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.", nameof(name));

            Name = name.Trim();
        }
    }
}
