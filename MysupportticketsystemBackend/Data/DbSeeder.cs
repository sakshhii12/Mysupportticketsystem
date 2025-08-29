using Microsoft.AspNetCore.Identity;
using MysupportticketsystemBackend.Models;

namespace MysupportticketsystemBackend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IApplicationBuilder app)
        {
            // Create a service scope to resolve the services we need
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // 1. Create Roles if they don't exist
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                if (!await roleManager.RoleExistsAsync("Agent"))
                    await roleManager.CreateAsync(new IdentityRole("Agent"));

                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));

                logger.LogInformation("Roles seeded successfully.");

                // 2. Create a default Admin user
                var adminEmail = "admin@example.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(adminUser, "AdminPassword123!");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Default Admin user created.");
                }

                // 3. Create a default Agent user
                var agentEmail = "agent@example.com";
                if (await userManager.FindByEmailAsync(agentEmail) == null)
                {
                    var agentUser = new ApplicationUser
                    {
                        UserName = agentEmail,
                        Email = agentEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(agentUser, "AgentPassword123!");
                    await userManager.AddToRoleAsync(agentUser, "Agent");
                    logger.LogInformation("Default Agent user created.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}