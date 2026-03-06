using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Data.Configurations
{
    public class TodoCategoryConfiguration : IEntityTypeConfiguration<TodoCategory>
    {
        public void Configure(EntityTypeBuilder<TodoCategory> builder)
        {
            builder.ToTable("TodoCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Color)
                .IsRequired()
                .HasMaxLength(7)
                .HasDefaultValue("#6366F1");

            builder.Property(x => x.Icon)
                .HasMaxLength(50);

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
