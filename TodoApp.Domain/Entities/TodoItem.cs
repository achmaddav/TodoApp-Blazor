using TodoApp.Domain.Common;
using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Entities
{
    public class TodoItem : AuditableEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public TodoStatus Status { get; private set; } = TodoStatus.Pending;
        public TodoPriority Priority { get; private set; } = TodoPriority.Medium;
        public DateTime? DueDate { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public int DisplayOrder { get; private set; }
        public string? Tags { get; private set; } // comma-separated

        // Foreign Keys
        public Guid? CategoryId { get; private set; }
        public Guid UserId { get; private set; }

        // Navigation Properties
        public TodoCategory? Category { get; private set; }
        public User? User { get; private set; }

        // EF Core constructor
        private TodoItem() { }

        public TodoItem(string title, Guid userId, string? description = null,
                        TodoPriority priority = TodoPriority.Medium,
                        DateTime? dueDate = null, Guid? categoryId = null)
        {
            SetTitle(title);
            UserId = userId;
            Description = description;
            Priority = priority;
            DueDate = dueDate;
            CategoryId = categoryId;
            Status = TodoStatus.Pending;
        }

        // ─── Domain Methods ───────────────────────────────────────────

        public void UpdateDetails(string title, string? description,
                                  TodoPriority priority, DateTime? dueDate,
                                  Guid? categoryId, string? tags)
        {
            SetTitle(title);
            Description = description;
            Priority = priority;
            DueDate = dueDate;
            CategoryId = categoryId;
            Tags = tags;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsInProgress()
        {
            if (Status == TodoStatus.Completed)
                throw new InvalidOperationException("Cannot reopen a completed todo.");

            Status = TodoStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsCompleted()
        {
            if (Status == TodoStatus.Cancelled)
                throw new InvalidOperationException("Cannot complete a cancelled todo.");

            Status = TodoStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == TodoStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed todo.");

            Status = TodoStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reopen()
        {
            Status = TodoStatus.Pending;
            CompletedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDisplayOrder(int order)
        {
            DisplayOrder = order;
        }

        public void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedBy = deletedBy;
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

            Title = title.Trim();
        }
    }
}
