using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Concrete
{
    public static class SeedRoles
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Tech", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            
            
        }
    }


}
