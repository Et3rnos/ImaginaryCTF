﻿using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
using iCTF_Shared_Resources.Models;
using iCTF_Website.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace iCTF_Website
{
    public static class DatabaseInitializer
    {
        public static void SeedUsers(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            //Log.Warning(context.Challenges.Where(x => x.State == 2).Include(x => x.Solves.OrderByDescending(x => x.SolvedAt).Take(1)).ThenInclude(x => x.User).ThenInclude(x => x.Team).ToQueryString());
            context.Database.Migrate();

            var roles = new string[] { "Administrator", "13:37 Hacker", "Booster", "Beta Tester", "Challenge Creator" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    roleStore.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpper() }).Wait();
                }
            }

            if (userManager.FindByNameAsync("admin").Result == null) {
                var player = new User();
                context.Players.Add(player);
                context.SaveChanges();

                var user = new ApplicationUser {
                    UserName = "admin",
                    Email = "a@b.c",
                    NormalizedEmail = "A@B.C",
                    User = player,
                    EmailConfirmed = true,
                    ApiKey = RandomHelper.GenerateRandomString()
                };

                context.SaveChanges();

                var result = userManager.CreateAsync(user, "Admin123!").Result;

                if (result.Succeeded) {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }
    }
}
