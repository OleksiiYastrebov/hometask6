using hometask6.Models;
using Microsoft.EntityFrameworkCore;

namespace hometask6.Data;

public class CategorySeeder
{
    public static async Task SeedAsync(ApplicationContext context)
    {
        if (!await context.Categories.AnyAsync())
        {
            var defaultCategories = new[]
            {
                new Category { Id = Guid.CreateVersion7(), Name = "Food" },
                new Category { Id = Guid.CreateVersion7(), Name = "Transport" },
                new Category { Id = Guid.CreateVersion7(), Name = "Mobile" },
                new Category { Id = Guid.CreateVersion7(), Name = "Internet" },
                new Category { Id = Guid.CreateVersion7(), Name = "Entertainment" }
            };

            context.Categories.AddRange(defaultCategories);
            await context.SaveChangesAsync();
        }
    }
}