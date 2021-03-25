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

            Config config = dbContext.Configuration.FirstOrDefault();
            if (config == null || config.ChallengeReleaseChannelId == 0)
            {
                return;
            }

            SocketTextChannel channel = client.GetGuild(config.GuildId).GetTextChannel(config.ChallengeReleaseChannelId);

            var lastChall = await dbContext.Challenges.AsAsyncEnumerable().Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync();

            Challenge chall = ChallengesManager.GetChallengeToBeReleased(dbContext, release: true).GetAwaiter().GetResult();
            
            if (chall == null)
            {
                return;
            }

            if (config.TodaysChannelId != 0 && config.TodaysRoleId != 0)
            {
                if (lastChall != null)
                {
                    var role = client.GetGuild(config.GuildId).GetRole(config.TodaysRoleId);
                    var userIds = await dbContext.Solves.AsAsyncEnumerable().Where(x => x.ChallengeId == lastChall.Id).Select(x => x.UserId).ToListAsync();
                    var discordIds = await dbContext.Users.AsAsyncEnumerable().Where(x => userIds.Contains(x.Id) && x.DiscordId != 0).Select(x => x.DiscordId).ToListAsync();
                    foreach (var discordId in discordIds)
                    {
                        await client.GetGuild(config.GuildId).GetUser(discordId).RemoveRoleAsync(role);
                    }
                }
                var todaysChannel = client.GetGuild(config.GuildId).GetTextChannel(config.TodaysChannelId);
                var messages = await todaysChannel.GetMessagesAsync(Int32.MaxValue).FlattenAsync();
                await todaysChannel.DeleteMessagesAsync(messages);
            }

            Embed embed = ChallengesManager.GetChallengeEmbed(chall);
            await channel.SendMessageAsync(embed: embed);
        }
    }
}
