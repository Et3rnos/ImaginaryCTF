using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCTF_Website
{
    public static class DatabaseInitializer
    {
        public static void SeedUsers(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            context.Database.Migrate();

            if (userManager.FindByNameAsync("admin").Result == null)
            {
                var player = new User();
                player.WebsiteUsername = "admin";
                context.Users.Add(player);
                context.SaveChanges();

                ApplicationUser user = new ApplicationUser
                {
                    UserName = "admin",
                    UserId = player.Id
                };

                context.SaveChanges();

                IdentityResult result = userManager.CreateAsync(user, "Admin123!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }
    }
}
