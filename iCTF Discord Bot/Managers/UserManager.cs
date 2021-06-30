﻿using Discord.WebSocket;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot.Managers
{
    class UserManager
    {
        public static async Task<User> GetOrCreateUser(DatabaseContext context, ulong discordId, string username)
        {
            var user = await context.Users.Include(x => x.SolvedChallenges).Include(x => x.WebsiteUser).Include(x => x.Team).ThenInclude(x => x.SolvedChallenges).Where(x => x.DiscordId == discordId).FirstOrDefaultAsync();
            if (user == null)
            {
                user = new User
                {
                    DiscordId = discordId,
                    DiscordUsername = username,
                    Score = 0,
                    LastUpdated = DateTime.UtcNow
                };
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
            return user;
        }

        public static string GetUsernameFromId(DiscordSocketClient client, ulong id)
        {
            var user = client.GetUser(id);
            return user != null ? user.Username : "invalid-user";
        }
    }
}
