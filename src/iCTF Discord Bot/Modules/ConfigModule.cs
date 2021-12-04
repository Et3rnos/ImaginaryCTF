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

namespace iCTF_Discord_Bot.Modules
{
    [Name("config")]
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
        [Summary("Links this bot to this server")]
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
            await ReplyAsync(embed: new EmbedBuilder().WithDescription("This bot is now linked to this server.").Build());
        }

        [Command("config")]
        [RequireContext(ContextType.Guild)]
        [Summary("View the current configuration values")]
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

            if (config.BoardWriteupsChannelId == 0)
                embed.Description += $"**Board Writeups Channel:** None\n";
            else
                embed.Description += $"**Board Writeups Channel:** <#{config.BoardWriteupsChannelId}>\n";

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
        [Summary("Sets the released challenges channel")]
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

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Challenge release channel set to: <#{channel.Id}>").Build());
        }

        [Command("setchallsolveschannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the solves announcement channel")]
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

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Challenge solves announcement channel set to: <#{channel.Id}>").Build());
        }

        [Command("setleaderboardchannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the leaderboard channel")]
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

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Leaderboard channel set to: <#{channel.Id}>").Build());
        }

        [Command("settodayschannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the today's channel")]
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
            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Today's channel set to: <#{channel.Id}>").Build());
        }

        [Command("setlogschannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the logs channel")]
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

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Logs channel set to: <#{channel.Id}>").Build());
        }

        [Command("setboardchannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the board channel")]
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

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Board channel set to: <#{channel.Id}>").Build());
        }

        [Command("setboardwriteupschannel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the board writeups channel")]
        public async Task SetBoardWriteupsChannel(IChannel channel = null)
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

            config.BoardWriteupsChannelId = channel.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Board writeups channel set to: <#{channel.Id}>").Build());
        }

        [Command("setboardrole")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the board role")]
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

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Board's role set to: {role.Mention}").Build());
        }

        [Command("settoproles")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the top roles")]
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

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Top roles set to: {first.Mention}, {second.Mention}, {third.Mention}").Build());
        }

        [Command("settodaysrole")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the today's role")]
        public async Task SetTodaysRole(IRole role)
        {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            config.TodaysRoleId = role.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Today's role set to: {role.Mention}").Build());
        }

        [Command("setchallengepingrole")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the challenge ping role")]
        public async Task SetChallengePingRole(IRole role) {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null) {
                await Link();
                config = await _context.Configuration.FirstOrDefaultAsync();
            }

            config.ChallengePingRoleId = role.Id;
            await _context.SaveChangesAsync();

            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Challenge ping role set to: {role.Mention}").Build());
        }

        [Command("setreleasetime")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Summary("Sets the release time (UTC)")]
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
            await ReplyAsync(embed: new EmbedBuilder().WithDescription($"Release time set to: **{(hours < 10 ? "0" + hours : hours)}H{(minutes < 10 ? "0" + minutes : minutes)} UTC**").Build());
        }
    }
}
