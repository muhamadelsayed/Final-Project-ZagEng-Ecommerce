using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // تأكد إن الجداول موجودة
        await context.Database.MigrateAsync();

        // لو مفيش Admin، أنشئ واحد
        var adminExists = await context.Users
            .AnyAsync(u => u.Email == "admin@ecommerce.com");

        if (!adminExists)
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Email = "admin@ecommerce.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "admin",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();

            Console.WriteLine("========================================");
            Console.WriteLine("Default Admin created:");
            Console.WriteLine("Email: admin@ecommerce.com");
            Console.WriteLine("Password: Admin@123");
            Console.WriteLine("========================================");
        }
    }
}