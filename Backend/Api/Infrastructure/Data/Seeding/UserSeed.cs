using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Seeding
{
    public static class UserSeed
    {
        public static async Task SeedUsersAndRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // === Create Roles ===
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // === Create Admin ===
            if (await userManager.FindByEmailAsync("admin@activigo.se") == null)
            {
                var admin = new User
                {
                    UserName = "admin@activigo.se",
                    Email = "admin@activigo.se",
                    Firstname = "System",
                    Lastname = "Admin", // om du har LastName i modellen
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!"); // lösenord
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
               
            }

            // === Create Test Users ===
            var testUsers = new List<User>
            {
                new User { UserName = "Anna@activigo.se", Email = "Anna@activigo.se", Firstname = "Anna", Lastname = "Andersson", EmailConfirmed = true },
                new User { UserName = "Bjorn@activigo.se", Email = "Bjorn@activigo.se", Firstname = "Bjorn", Lastname = "Berg", EmailConfirmed = true },
                new User { UserName = "Carla@activigo.se", Email = "Carla@activigo.se", Firstname = "Carla", Lastname = "Carlsson", EmailConfirmed = true }
            };

            foreach (var user in testUsers)
            {               
                if (await userManager.FindByEmailAsync(user.Email) == null)
                {
                   var result = await userManager.CreateAsync(user, "User123!"); // samma lösen för enkelhet
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                   
                }
            }
        }
    }
}
