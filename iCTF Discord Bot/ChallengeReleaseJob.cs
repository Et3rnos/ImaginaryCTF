using Discord;
using Discord.WebSocket;
using iCTF_Discord_Bot.Managers;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot
{
    class ChallengeReleaseJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            SchedulerContext schedulerContext = context.Scheduler.Context;
            DiscordSocketClient client = (DiscordSocketClient)schedulerContext.Get("client");
            IServiceScopeFactory scopeFactory = (IServiceScopeFactory)schedulerContext.Get("scopeFactory");
            
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DatabaseContext>();

            var config = dbContext.Configuration.FirstOrDefault();
            if (config == null || config.ChallengeReleaseChannelId == 0) {
                return;
            }

            var channel = client.GetGuild(config.GuildId).GetTextChannel(config.ChallengeReleaseChannelId);

            var lastChall = await dbContext.Challenges.AsAsyncEnumerable().Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync();

            var chall = ChallengesManager.GetChallengeToBeReleased(dbContext, release: true).GetAwaiter().GetResult();
            
            if (chall == null) {
                return;
            }

            if (config.TodaysChannelId != 0 && config.TodaysRoleId != 0) {
                if (lastChall != null) {
                    var oldRole = client.GetGuild(config.GuildId).GetRole(config.TodaysRoleId);
                    var newRole = await client.GetGuild(config.GuildId).CreateRoleAsync(oldRole.Name, oldRole.Permissions, oldRole.Color, oldRole.IsHoisted, oldRole.IsMentionable);
                    await client.GetGuild(config.GuildId).GetTextChannel(config.TodaysChannelId).AddPermissionOverwriteAsync(newRole, new OverwritePermissions(viewChannel: PermValue.Allow));
                    await oldRole.DeleteAsync();
                    config.TodaysRoleId = newRole.Id;
                    await dbContext.SaveChangesAsync();
                }
                var todaysChannel = client.GetGuild(config.GuildId).GetTextChannel(config.TodaysChannelId);
                var messages = await todaysChannel.GetMessagesAsync(int.MaxValue).FlattenAsync();
                messages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
                if (messages.Any()) {
                    await todaysChannel.DeleteMessagesAsync(messages);
                }
                if (!string.IsNullOrEmpty(chall.Writeup)) {
                    var writeupEmbed = new CustomEmbedBuilder();
                    writeupEmbed.WithTitle("Intended Solution");
                    writeupEmbed.WithDescription(chall.Writeup);
                    var message = await todaysChannel.SendMessageAsync(embed: writeupEmbed.Build());
                    await message.PinAsync();
                }
            }

            var embed = ChallengesManager.GetChallengeEmbed(chall);
            if (config.ChallengePingRoleId != 0) {
                await channel.SendMessageAsync($"<@&{config.ChallengePingRoleId}>", embed: embed);
            } else {
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}
