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

            var rolesToRemove = new List<SocketRole> { firstRole, secondRole, thirdRole };

            var users = await SharedLeaderboardManager.GetTopUsersAndTeams(context, 5);

            var top = users.FirstOrDefault();
            if (top != null && top.DiscordId != 0)
            {
                var topUser = client.GetGuild(config.GuildId).GetUser(top.DiscordId);
                if (topUser != null && !topUser.Roles.Any(x => x.Id == firstRole.Id))
                    await topUser.SendMessageAsync("Congrats for reaching the top of the leaderboard :partying_face:");
            }

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                if (user.DiscordId == 0) continue;

                var dUser = client.GetGuild(config.GuildId).GetUser(user.DiscordId);
                if (dUser == null) continue;

                switch (i)
                {
                    case 0:
                        if (!dUser.Roles.Any(x => x.Id == firstRole.Id))
                        {
                            await dUser.RemoveRolesAsync(rolesToRemove);
                            await dUser.AddRoleAsync(firstRole);
                        }
                        break;
                    case 1:
                        if (!dUser.Roles.Any(x => x.Id == secondRole.Id))
                        {
                            await dUser.RemoveRolesAsync(rolesToRemove);
                            await dUser.AddRoleAsync(secondRole);
                        }
                        break;
                    case 2:
                        if (!dUser.Roles.Any(x => x.Id == thirdRole.Id))
                        {
                            await dUser.RemoveRolesAsync(rolesToRemove);
                            await dUser.AddRoleAsync(thirdRole);
                        }
                        break;
                    default:
                        await dUser.RemoveRolesAsync(rolesToRemove);
                        break;
                }
            }
        }
    }
}
