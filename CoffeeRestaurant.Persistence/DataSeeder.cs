using CoffeeRestaurant.Domain.Entities;
using CoffeeRestaurant.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoffeeRestaurant.Persistence;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoffeeDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CoffeeDbContext>>();

        try
        {
            // Seed Roles
            await SeedRolesAsync(roleManager);
            
            // Seed Categories
            await SeedCategoriesAsync(context);
            
            // Seed Coffee Items
            await SeedCoffeeItemsAsync(context);
            
            // Seed Admin User
            await SeedAdminUserAsync(userManager);
            
            logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding data.");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "Barista", "Customer" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedCategoriesAsync(CoffeeDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new[]
        {
            new Category { Id = Guid.NewGuid(), Name = "Espresso", Description = "Strong coffee shots" },
            new Category { Id = Guid.NewGuid(), Name = "Cappuccino", Description = "Espresso with steamed milk" },
            new Category { Id = Guid.NewGuid(), Name = "Latte", Description = "Espresso with lots of milk" },
            new Category { Id = Guid.NewGuid(), Name = "Americano", Description = "Espresso with hot water" },
            new Category { Id = Guid.NewGuid(), Name = "Mocha", Description = "Espresso with chocolate" },
            new Category { Id = Guid.NewGuid(), Name = "Tea", Description = "Various tea options" },
            new Category { Id = Guid.NewGuid(), Name = "Pastries", Description = "Fresh baked goods" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCoffeeItemsAsync(CoffeeDbContext context)
    {
        if (await context.CoffeeItems.AnyAsync())
            return;

        var categories = await context.Categories.ToListAsync();
        var espressoCategory = categories.First(c => c.Name == "Espresso");
        var cappuccinoCategory = categories.First(c => c.Name == "Cappuccino");
        var latteCategory = categories.First(c => c.Name == "Latte");
        var americanoCategory = categories.First(c => c.Name == "Americano");
        var mochaCategory = categories.First(c => c.Name == "Mocha");
        var teaCategory = categories.First(c => c.Name == "Tea");
        var pastriesCategory = categories.First(c => c.Name == "Pastries");

        var coffeeItems = new[]
        {
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Single Espresso", Description = "Strong single shot", Price = 2.50m, CategoryId = espressoCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Double Espresso", Description = "Strong double shot", Price = 3.50m, CategoryId = espressoCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Classic Cappuccino", Description = "Perfect balance of espresso and milk", Price = 4.00m, CategoryId = cappuccinoCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Vanilla Cappuccino", Description = "Cappuccino with vanilla flavor", Price = 4.50m, CategoryId = cappuccinoCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Caramel Latte", Description = "Smooth latte with caramel", Price = 4.75m, CategoryId = latteCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Classic Americano", Description = "Espresso with hot water", Price = 3.00m, CategoryId = americanoCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Dark Chocolate Mocha", Description = "Rich mocha with dark chocolate", Price = 5.00m, CategoryId = mochaCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Green Tea", Description = "Refreshing green tea", Price = 2.50m, CategoryId = teaCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Croissant", Description = "Buttery French croissant", Price = 3.50m, CategoryId = pastriesCategory.Id },
            new CoffeeItem { Id = Guid.NewGuid(), Name = "Blueberry Muffin", Description = "Fresh baked muffin", Price = 3.00m, CategoryId = pastriesCategory.Id }
        };

        await context.CoffeeItems.AddRangeAsync(coffeeItems);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "admin@coffeerestaurant.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
