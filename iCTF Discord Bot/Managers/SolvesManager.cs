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
using iCTF_Shared_Resources.Models;

namespace iCTF_Discord_Bot
{
    class SolvesManager
    {

        public static async Task AnnounceSolve(DiscordSocketClient client, DatabaseContext context, Challenge challenge, User user)
        {
            Config config = await context.Configuration.FirstOrDefaultAsync();
            if (config == null || config.GuildId == 0 || config.ChallengeSolvesChannelId == 0)
            {
                return;
            }
            SocketTextChannel channel =  client.GetGuild(config.GuildId).GetTextChannel(config.ChallengeSolvesChannelId);
            await channel.SendMessageAsync($"<@{user.DiscordId}> solved **{challenge.Title}** challenge!");
        }

        public static async Task AnnounceWebsiteSolves(DiscordSocketClient client, DatabaseContext context)
        {
            var solves = await context.Solves.AsAsyncEnumerable().Where(x => x.Announced == false).ToListAsync();
            if (solves.Count == 0)
            {
                return;
            }
            Config config = await context.Configuration.FirstOrDefaultAsync();
            if (config == null && config.GuildId != 0 && config.ChallengeSolvesChannelId != 0)
            {
                return;
            }
            SocketTextChannel channel = client.GetGuild(config.GuildId).GetTextChannel(config.ChallengeSolvesChannelId);

            foreach (var solve in solves)
            {
                var user = await context.Users.AsAsyncEnumerable().Where(x => x.Id == solve.UserId).FirstOrDefaultAsync();
                if (config.TodaysRoleId != 0 && user.DiscordId != 0)
                {
                    var lastChall = await context.Challenges.AsAsyncEnumerable().Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync();
                    if (lastChall.Id == solve.ChallengeId)
                    {
                        var guildUser = client.GetGuild(config.GuildId).GetUser(user.DiscordId);
                        if (guildUser != null)
                        {
                            var role = client.GetGuild(config.GuildId).GetRole(config.TodaysRoleId);
                            await guildUser.AddRoleAsync(role);
                        }
                    }
                }
                await channel.SendMessageAsync($"**{solve.Username}** solved **{solve.ChallengeTitle}** challenge!");
                solve.Announced = true;
            }
            await context.SaveChangesAsync();
            await RolesManager.UpdateRoles(client, context);
            await LeaderboardManager.UpdateLeaderboard(client, context);
        }
    }
}
