using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Data.Seeders
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await SeedCategoriesAsync(context);
            await SeedAdminUserAsync(context);
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.TodoCategories.AnyAsync()) return;

            var categories = new List<TodoCategory>
        {
            new("Work",     "Work related tasks",     "#6366F1", "work"),
            new("Personal", "Personal tasks",          "#EC4899", "person"),
            new("Shopping", "Shopping list",           "#F59E0B", "shopping-cart"),
            new("Health",   "Health & fitness tasks",  "#10B981", "heart"),
            new("Learning", "Study & learning tasks",  "#3B82F6", "book"),
        };

            await context.TodoCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAdminUserAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var admin = new User(
                fullName: "Admin User",
                email: "admin@todoapp.com",
                passwordHash: BCrypt.Net.BCrypt.HashPassword("Admin@123")
            );

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
