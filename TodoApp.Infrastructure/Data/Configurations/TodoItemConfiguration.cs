using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Infrastructure.Data.Configurations
{
    public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.ToTable("TodoItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue(TodoStatus.Pending)
                .HasConversion<int>();

            builder.Property(x => x.Priority)
                .IsRequired()
                .HasDefaultValue(TodoPriority.Medium)
                .HasConversion<int>();

            builder.Property(x => x.Tags)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(x => x.Category)
                .WithMany(x => x.TodoItems)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.User)
                .WithMany(x => x.TodoItems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsDeleted);
            builder.HasIndex(x => new { x.UserId, x.IsDeleted });

            // Global Query Filter — auto exclude soft deleted
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
