using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using iCTF_Discord_Bot.Managers;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.Extensions.DependencyInjection;

namespace iCTF_Discord_Bot
{
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly DatabaseContext _context;
        private readonly IServiceScope _scope;

        private ConfigModule(DiscordSocketClient client, CommandService commands, IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _commands = commands;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
        }

        ~ConfigModule() { _scope.Dispose(); }

        [Command("link")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task Link()
        {
            switch (await LinkedGuildManager.GetLinkState(_context, Context.Guild.Id))
            {
                case LinkedGuildManager.LinkState.NotLinked:
                    Config config = new Config
                    {
                        GuildId = Context.Guild.Id
                    };
                    await _context.Configuration.AddAsync(config);
                    await _context.SaveChangesAsync();

                    await ReplyAsync("This bot is now linked to this server.");
                    break;
                case LinkedGuildManager.LinkState.LinkedToThisGuild:
                    await ReplyAsync("This bot is already linked to this server.");
                    break;
                case LinkedGuildManager.LinkState.LinkedToOtherGuild:
                    await ReplyAsync("This bot is already linked to another server.");
                    break;
            }
        }

        [Command("config")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task Config(IChannel channel = null) {
            if (!(await CanSetConfig())) {
                return;
            }
            Config config = await _context.Configuration.FirstOrDefaultAsync();
            var embed = new CustomEmbedBuilder();
            embed.WithTitle("Configuration");

            if (config.ChallengeReleaseChannelId == 0)
                embed.Description += $"**Challenge Release Channel:** None\n";
            else
                embed.Description += $"**Challenge Release Channel:** <#{config.ChallengeReleaseChannelId}>\n";

            if (config.ChallengeSolvesChannelId == 0)
                embed.Description += $"**Challenge Solves Channel:** None\n";
            else
                embed.Description += $"**Challenge Solves Channel:** <#{config.ChallengeSolvesChannelId}>\n";

            if (config.LeaderboardChannelId == 0)
                embed.Description += $"**Leaderboard Channel:** None\n";
            else
                embed.Description += $"**Leaderboard Channel:** <#{config.LeaderboardChannelId}>\n";

            if (config.TodaysChannelId == 0)
                embed.Description += $"**Today's Challenge Channel:** None\n";
            else
                embed.Description += $"**Today's Challenge Channel:** <#{config.TodaysChannelId}>\n";

            if (config.LogsChannelId == 0)
                embed.Description += $"**Logs Channel:** None\n";
            else
                embed.Description += $"**Logs Channel:** <#{config.LogsChannelId}>\n";

            if (config.FirstPlaceRoleId == 0 || config.SecondPlaceRoleId == 0 || config.ThirdPlaceRoleId == 0)
                embed.Description += $"**Top Roles:** None\n";
            else
                embed.Description += $"**Top Roles:** <@&{config.FirstPlaceRoleId}> <@&{config.SecondPlaceRoleId}> <@&{config.ThirdPlaceRoleId}>\n";

            if (config.TodaysRoleId == 0)
                embed.Description += $"**Today's Challenge Role:** None\n";
            else
                embed.Description += $"**Today's Challenge Role:** <@&{config.TodaysRoleId}>\n";

            if (config.ChallengePingRoleId == 0)
                embed.Description += $"**Challenge Ping Role:** None\n";
            else
                embed.Description += $"**Challenge Ping Role:** <@&{config.ChallengePingRoleId}>\n";

            embed.Description += $"**Release Time:** {config.ReleaseTime / 60}H{config.ReleaseTime % 60} UTC";
            await ReplyAsync(embed: embed.Build());
        }

        [Command("setchallreleasechannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetChallReleaseChannel(IChannel channel = null)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            if (channel == null)
            {
                channel = Context.Channel;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.ChallengeReleaseChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Challenge release channel set to: <#{channel.Id}>");
        }

        [Command("setchallsolveschannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetChallSolvesChannel(IChannel channel = null)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            if (channel == null)
            {
                channel = Context.Channel;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.ChallengeSolvesChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Challenge solves announcement channel set to: <#{channel.Id}>");
        }

        [Command("setleaderboardchannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetLeaderboardChannel(IChannel channel = null)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            if (channel == null)
            {
                channel = Context.Channel;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.LeaderboardChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Leaderboard channel set to: <#{channel.Id}>");
        }

        [Command("settodayschannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetTodaysChannel(IChannel channel = null)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            if (channel == null)
            {
                channel = Context.Channel;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.TodaysChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Today's channel set to: <#{channel.Id}>");
        }

        [Command("setlogschannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetLogsChannel(IChannel channel = null)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            if (channel == null)
            {
                channel = Context.Channel;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.LogsChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Logs channel set to: <#{channel.Id}>");
        }

        [Command("settoproles")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetTopRoles(IRole first, IRole second, IRole third)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.FirstPlaceRoleId = first.Id;
            config.SecondPlaceRoleId = second.Id;
            config.ThirdPlaceRoleId = third.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Top roles set to: {first.Mention}, {second.Mention}, {third.Mention}");
        }

        [Command("settodaysrole")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetTodaysRole(IRole role)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.TodaysRoleId = role.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Today's role set to: {role.Mention}");
        }

        [Command("setchallengepingrole")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetChallengePingRole(IRole role) {
            if (!(await CanSetConfig())) {
                return;
            }

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.ChallengePingRoleId = role.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Challenge ping role set to: {role.Mention}");
        }

        [Command("setreleasetime")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetReleaseTime(uint hours, uint minutes)
        {
            if (!(await CanSetConfig()))
            {
                return;
            }

            uint time = hours * 60 + minutes;

            Config config = await _context.Configuration.FirstOrDefaultAsync();
            config.ReleaseTime = time;
            await _context.SaveChangesAsync();

            Scheduler.UpdateChallengeReleaseJob(config);
            await ReplyAsync($"Release time set to: **{hours}H{minutes} UTC**");
        }

        private async Task<bool> CanSetConfig()
        {
            switch (await LinkedGuildManager.GetLinkState(_context, Context.Guild.Id))
            {
                case LinkedGuildManager.LinkState.NotLinked:
                    await ReplyAsync("This bot is not linked to a server. Please run `.link` first.");
                    return false;
                case LinkedGuildManager.LinkState.LinkedToOtherGuild:
                    await ReplyAsync("This bot is linked to another server. You cannot use this command.");
                    return false;
                default:
                    return true;
            }
        }
    }
}
