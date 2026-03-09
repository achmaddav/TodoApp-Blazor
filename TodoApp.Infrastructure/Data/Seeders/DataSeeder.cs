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

        private static async Task SeedCategoriesAsync(AppDbContext context, bool force = false)
        {
            if (!force && await context.TodoCategories.AnyAsync()) return;

            // Hapus semua jika force
            if (force)
            {
                context.TodoCategories.RemoveRange(context.TodoCategories);
                await context.SaveChangesAsync();
            }

            var categories = new List<TodoCategory>
            {
                new("Work",     "Work & office tasks",        "#6366F1", "work"),
                new("Personal", "Personal activities",         "#EC4899", "person"),
                new("Shopping", "Shopping list",               "#F59E0B", "shopping_cart"),
                new("Health",   "Health & fitness",            "#10B981", "favorite"),
                new("Learning", "Study & education",           "#3B82F6", "menu_book"),
                new("Finance",  "Finance & bills",             "#14B8A6", "attach_money"),
                new("Home",     "Household tasks",             "#8B5CF6", "home"),
                new("Social",   "Social events & activities",  "#EF4444", "group"),
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
