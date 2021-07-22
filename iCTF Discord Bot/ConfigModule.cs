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
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                config = new Config {
                    GuildId = Context.Guild.Id
                };
                await _context.Configuration.AddAsync(config);
            } else {
                config.GuildId = Context.Guild.Id;
            }
            await _context.SaveChangesAsync();
            await ReplyAsync("This bot is now linked to this server.");
        }

        [Command("config")]
        [RequireContext(ContextType.Guild)]
        public async Task Config() {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

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

            if (config.BoardChannelId == 0)
                embed.Description += $"**Board's Channel:** None\n";
            else
                embed.Description += $"**Board's Channel:** <#{config.BoardChannelId}>\n";

            if (config.BoardRoleId == 0)
                embed.Description += $"**Board's Role:** None\n";
            else
                embed.Description += $"**Board's Role:** <@&{config.BoardRoleId}>\n";

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
        public async Task SetChallReleaseChannel(IChannel channel = null) {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            if (channel == null) {
                channel = Context.Channel;
            }

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
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            if (channel == null) {
                channel = Context.Channel;
            }

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
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            if (channel == null) {
                channel = Context.Channel;
            }

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
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            if (channel == null) {
                channel = Context.Channel;
            }

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
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            if (channel == null) {
                channel = Context.Channel;
            }

            config.LogsChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Logs channel set to: <#{channel.Id}>");
        }

        [Command("setboardchannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetBoardChannel(IChannel channel = null)
        {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null)
            {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            if (channel == null)
            {
                channel = Context.Channel;
            }

            config.BoardChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Board channel set to: <#{channel.Id}>");
        }

        [Command("setboardrole")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetBoardRole(IRole role)
        {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null)
            {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            config.BoardRoleId = role.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Board's role set to: {role.Mention}");
        }

        [Command("settoproles")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetTopRoles(IRole first, IRole second, IRole third)
        {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

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
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            config.TodaysRoleId = role.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Today's role set to: {role.Mention}");
        }

        [Command("setchallengepingrole")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetChallengePingRole(IRole role) {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            config.ChallengePingRoleId = role.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync($"Challenge ping role set to: {role.Mention}");
        }

        [Command("setreleasetime")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task SetReleaseTime(int hours, int minutes)
        {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            int time = hours * 60 + minutes;

            config.ReleaseTime = time;
            await _context.SaveChangesAsync();

            Scheduler.UpdateChallengeReleaseJob(config);
            await ReplyAsync($"Release time set to: **{(hours < 10 ? "0" + hours : hours)}H{(minutes < 10 ? "0" + minutes : minutes)} UTC**");
        }
    }
}
