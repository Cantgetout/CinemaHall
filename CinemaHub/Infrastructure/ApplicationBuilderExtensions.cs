using Microsoft.AspNetCore.Identity;
using CinemaHub.Data;
// Make sure you include the namespace where your ApplicationUser lives!
using CinemaHub.Data.Models;
using CinemaHub.Data.Seeders;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> PrepareDatabase(this IApplicationBuilder app, IConfiguration config)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<Program>>();

            // IMPORTANT: Changed IdentityUser to ApplicationUser
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                logger.LogInformation("Starting database seeding process...");

                // Извикваме DbSeeder и му подаваме конфигурацията, за да може да прочете тайните пароли
                await DbSeeder.SeedDataAsync(services, config);

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }

            return app;
        }
    }
}