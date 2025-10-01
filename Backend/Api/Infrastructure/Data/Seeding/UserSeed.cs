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
                    UserName = "admin123",
                    Email = "admin@activigo.se",
                    Firstname = "System",
                    Lastname = "Admin", // om du har LastName i modellen
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "Admin123!"); // lösenord
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // === Create Test Users ===
            var testUsers = new List<User>
            {
                new User { UserName = "Anna1@activigo.se", Email = "Anna@activigo.se", Firstname = "Anna", Lastname = "Andersson", EmailConfirmed = true },
                new User { UserName = "Björn2@activigo.se", Email = "Björn@activigo.se", Firstname = "Björn", Lastname = "Berg", EmailConfirmed = true },
                new User { UserName = "Carla3@activigo.se", Email = "Carla@activigo.se", Firstname = "Carla", Lastname = "Carlsson", EmailConfirmed = true }
            };

            foreach (var user in testUsers)
            {
                if (await userManager.FindByEmailAsync(user.Email) == null)
                {
                    await userManager.CreateAsync(user, "User123!"); // samma lösen för enkelhet
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}
