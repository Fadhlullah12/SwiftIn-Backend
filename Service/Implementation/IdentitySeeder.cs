using Microsoft.AspNetCore.Identity;
using SwiftIn.Models.Entities;
using SwiftIn.Settings;

namespace SwiftIn.Services
{
    // Creates the roles and the very first admin, otherwise nobody could ever log in to create others.
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in AppRoles.All)
            {
                if (!await roles.RoleExistsAsync(role))
                    await roles.CreateAsync(new IdentityRole(role));
            }

            var email = config["SeedAdmin:Email"];
            var password = config["SeedAdmin:Password"];
            var fullName = config["SeedAdmin:FullName"] ?? "System Admin";

            // No seed settings = nothing to create (fine for later runs)
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            if (await users.FindByEmailAsync(email) is not null)
                return;

            var admin = new ApplicationUser { UserName = email, Email = email, FullName = fullName };
            var result = await users.CreateAsync(admin, password);
            if (result.Succeeded)
                await users.AddToRoleAsync(admin, AppRoles.Admin);
        }
    }
}