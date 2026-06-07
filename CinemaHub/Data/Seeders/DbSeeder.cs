using CinemaHub.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CinemaHub.Data.Seeders
{
    public static class DbSeeder    //used ai to help 
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider, IConfiguration config)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // 1. Създаване на роли
            string[] roleNames = { "Administrator", "Moderator", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Създаване на Master Admin
            string adminEmail = "admin@cinemahub.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                // Взимаме админ паролата от Secret Manager (или ползваме fallback)
                var adminPassword = config["SeederConfiguration:AdminPassword"] ?? "SuperSecretAdmin123!";
                var result = await userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Administrator");
                }
            }

            // 3. Създаване на Dummy Users (Ако в базата има само 1 потребител - админа)
            if (userManager.Users.Count() <= 1)
            {
                // Взимаме dummy паролата от Secret Manager
                var dummyPassword = config["SeederConfiguration:DummyUserPassword"];

                // Fail-Safe защита: Ако няма парола в конфигурацията, спираме процеса!
                if (string.IsNullOrEmpty(dummyPassword))
                {
                    logger.LogWarning("Dummy password not found in configuration. Skipping dummy user seeding to prevent insecure defaults.");
                    return;
                }

                logger.LogInformation("Seeding 120 dummy accounts...");

                // Създаване на 20 Модератори
                for (int i = 1; i <= 20; i++)
                {
                    var modEmail = $"mod{i}@cinemahub.com";
                    var modUser = new ApplicationUser
                    {
                        UserName = modEmail,
                        Email = modEmail,
                        FirstName = "Moderator",
                        LastName = $"#{i}",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(modUser, dummyPassword);
                    if (result.Succeeded) await userManager.AddToRoleAsync(modUser, "Moderator");
                }

                // Създаване на 100 Потребители
                for (int i = 1; i <= 100; i++)
                {
                    var userEmail = $"user{i}@cinemahub.com";
                    var normalUser = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        FirstName = "TestUser",
                        LastName = $"#{i}",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(normalUser, dummyPassword);
                    if (result.Succeeded) await userManager.AddToRoleAsync(normalUser, "User");
                }
            }
        }
    }
}