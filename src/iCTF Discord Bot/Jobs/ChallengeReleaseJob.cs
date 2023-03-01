using Discord;
using Discord.Rest;
using Discord.WebSocket;
using iCTF_Discord_Bot.Managers;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot.Jobs
{
    class ChallengeReleaseJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("Starting Challenge Release Job");

            SchedulerContext schedulerContext = context.Scheduler.Context;
            DiscordSocketClient client = (DiscordSocketClient)schedulerContext.Get("client");
            IServiceScopeFactory scopeFactory = (IServiceScopeFactory)schedulerContext.Get("scopeFactory");
            
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DatabaseContext>();

            var config = dbContext.Configuration.FirstOrDefault();
            if (config == null || config.ChallengeReleaseChannelId == 0)
            {
                Log.Information("Aborting Challenge Release Job because some configuration values are not defined");
                return;
            }

            var channel = client.GetGuild(config.GuildId).GetTextChannel(config.ChallengeReleaseChannelId);

            var lastChall = await dbContext.Challenges.AsAsyncEnumerable().Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync();

            var chall = ChallengesManager.GetChallengeToBeReleased(dbContext, release: true).GetAwaiter().GetResult();
            
            if (chall == null)
            {
                Log.Information("Aborting Challenge Release Job because no challenge is ready to be released");
                return;
            }

            var challengeChannelsCategory = client.GetGuild(config.GuildId).CategoryChannels.FirstOrDefault(x => x.Name.ToLower() == "challenge channels");
            if (challengeChannelsCategory != null)
            {
                var challengeRole = await client.GetGuild(config.GuildId).CreateRoleAsync("solved-" + chall.Title);
                var channelPermissions = new List<Overwrite> { 
                    new Overwrite(challengeRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Allow)),
                    new Overwrite(client.GetGuild(config.GuildId).EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny))
                };
                var challengeChannel = await client.GetGuild(config.GuildId).CreateTextChannelAsync(chall.Title, tcp => { tcp.CategoryId = challengeChannelsCategory.Id; tcp.PermissionOverwrites = channelPermissions; });

                if (!string.IsNullOrEmpty(chall.Writeup))
                {
                    var writeupEmbed = new CustomEmbedBuilder();
                    writeupEmbed.WithTitle("Intended Solution");
                    writeupEmbed.WithDescription(chall.Writeup);
                    var message = await challengeChannel.SendMessageAsync(embed: writeupEmbed.Build());
                    await message.PinAsync();
                }
            }

            if (!string.IsNullOrEmpty(chall.Writeup) && config.BoardWriteupsChannelId != 0)
            {
                Log.Information("Posting the challenge writeup on the board writeups channel");
                var writeupEmbed = new CustomEmbedBuilder();
                writeupEmbed.WithTitle($"Writeup for {chall.Title}");
                writeupEmbed.WithDescription($"{chall.Writeup}\n**Flag:** {chall.Flag}");
                var writeupsChannel = client.GetGuild(config.GuildId).GetTextChannel(config.BoardWriteupsChannelId);
                await writeupsChannel.SendMessageAsync(embed: writeupEmbed.Build());
            }

            Log.Information("Posting the challenge on Discord");
            var embed = ChallengesManager.GetChallengeEmbed(chall);
            RestUserMessage challengeMessage;
            if (config.ChallengePingRoleId != 0) {
                challengeMessage = await channel.SendMessageAsync($"<@&{config.ChallengePingRoleId}>", embed: embed);
            } else {
                challengeMessage = await channel.SendMessageAsync(embed: embed);
            }
            if (channel is INewsChannel) await challengeMessage.CrosspostAsync();

            Log.Information("Ending Challenge Release Job");
        }
    }
}
