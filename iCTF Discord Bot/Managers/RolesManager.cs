using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using iCTF_Discord_Bot.Managers;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
using iCTF_Shared_Resources.Models;

namespace iCTF_Discord_Bot
{
    class RolesManager
    {
        public static async Task UpdateRoles(DiscordSocketClient client, DatabaseContext context)
        {
            Config config = await context.Configuration.FirstOrDefaultAsync();
            if (config == null || config.GuildId == 0 || config.FirstPlaceRoleId == 0 || config.SecondPlaceRoleId == 0 || config.ThirdPlaceRoleId == 0)
            {
                return;
            }

            var firstRole = client.GetGuild(config.GuildId).GetRole(config.FirstPlaceRoleId);
            var secondRole = client.GetGuild(config.GuildId).GetRole(config.SecondPlaceRoleId);
            var thirdRole = client.GetGuild(config.GuildId).GetRole(config.ThirdPlaceRoleId);

            if (firstRole == null || secondRole == null || thirdRole == null)
            {
                return;
            }

            List<SocketRole> rolesToRemove = new List<SocketRole>();
            rolesToRemove.Add(firstRole);
            rolesToRemove.Add(secondRole);
            rolesToRemove.Add(thirdRole);

            List<User> users = await SharedLeaderboardManager.GetTopPlayers(context, 5);

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                if (user.DiscordId == 0) { continue; }

                var dUser = client.GetGuild(config.GuildId).GetUser(user.DiscordId);
                await dUser.RemoveRolesAsync(rolesToRemove);
                switch (i)
                {
                    case 0:
                        await dUser.AddRoleAsync(firstRole);
                        break;
                    case 1:
                        await dUser.AddRoleAsync(secondRole);
                        break;
                    case 2:
                        await dUser.AddRoleAsync(thirdRole);
                        break;
                }
            }
        }
    }
}
