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
using Microsoft.EntityFrameworkCore;

namespace iCTF_Discord_Bot
{
    class SolvesManager
    {

        public static async Task AnnounceSolve(DiscordSocketClient client, DatabaseContext context, Challenge challenge, User user)
        {
            var config = await context.Configuration.AsQueryable().FirstOrDefaultAsync();
            if (config == null || config.GuildId == 0 || config.ChallengeSolvesChannelId == 0) {
                return;
            }
            var channel =  client.GetGuild(config.GuildId).GetTextChannel(config.ChallengeSolvesChannelId);
            await channel.SendMessageAsync($"<@{user.DiscordId}> solved **{challenge.Title}** challenge!");
        }

        public static async Task AnnounceWebsiteSolves(DiscordSocketClient client, DatabaseContext context, bool dynamicScoring = false)
        {
            var solves = await context.Solves.Include(x => x.User.WebsiteUser).Include(x => x.Challenge).Include(x => x.Team).ThenInclude(x => x.Members).Where(x => x.Announced == false).ToListAsync();
            if (solves.Count == 0) {
                return;
            }
            var config = await context.Configuration.AsQueryable().FirstOrDefaultAsync();
            if (config == null || config.GuildId == 0 || config.ChallengeSolvesChannelId == 0) {
                return;
            }
            var channel = client.GetGuild(config.GuildId).GetTextChannel(config.ChallengeSolvesChannelId);

            foreach (var solve in solves)
            {
                if (config.TodaysRoleId != 0 && solve.User.DiscordId != 0)
                {
                    var lastChall = await context.Challenges.AsAsyncEnumerable().Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync();
                    if (lastChall == solve.Challenge)
                    {
                        if (solve.Team != null)
                        {
                            foreach (var member in solve.Team.Members)
                            {
                                if (member.DiscordId == 0) continue;
                                var guildUser = client.GetGuild(config.GuildId).GetUser(member.DiscordId);
                                if (guildUser != null)
                                {
                                    var role = client.GetGuild(config.GuildId).GetRole(config.TodaysRoleId);
                                    await guildUser.AddRoleAsync(role);
                                }
                            }
                        }
                        else
                        {
                            var guildUser = client.GetGuild(config.GuildId).GetUser(solve.User.DiscordId);
                            if (guildUser != null)
                            {
                                var role = client.GetGuild(config.GuildId).GetRole(config.TodaysRoleId);
                                await guildUser.AddRoleAsync(role);
                            }
                        }
                    }
                }
                await channel.SendMessageAsync($"**{solve.User.WebsiteUser.UserName.Replace("@", "@\u200B")}** solved **{solve.Challenge.Title}** challenge!");
                solve.Announced = true;
            }
            await context.SaveChangesAsync();
            await RolesManager.UpdateRoles(client, context);
            await LeaderboardManager.UpdateLeaderboard(client, context, dynamicScoring);
        }
    }
}
