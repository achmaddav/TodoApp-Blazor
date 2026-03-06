using TodoApp.Domain.Common;

namespace TodoApp.Domain.Entities
{
    public class User : AuditableEntity
    {
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;
        public DateTime? LastLoginAt { get; private set; }

        // Navigation
        private readonly List<TodoItem> _todoItems = new();
        public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();

        private User() { }

        public User(string fullName, string email, string passwordHash)
        {
            SetEmail(email);
            SetFullName(fullName);
            PasswordHash = passwordHash;
        }

        public void UpdateProfile(string fullName)
        {
            SetFullName(fullName);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        private void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty.");
            FullName = fullName.Trim();
        }

        private void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");
            Email = email.Trim().ToLower();
        }
    }
}
