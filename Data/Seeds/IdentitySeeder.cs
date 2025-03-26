using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AccessControl.Seeding
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            var roleName = "Admin";

            var exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists && !roleManager.Roles.Any(r => r.NormalizedName == roleName.ToUpper()))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var adminEmail = "admin@access.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(user, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName); 
                }
            }
        }
    }

}
