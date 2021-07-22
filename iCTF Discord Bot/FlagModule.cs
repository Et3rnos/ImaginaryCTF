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
using iCTF_Shared_Resources.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace iCTF_Discord_Bot
{
    public class FlagModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly DatabaseContext _context;
        private readonly IServiceScope _scope;

        public FlagModule(DiscordSocketClient client, CommandService commands, IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _commands = commands;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
        }

        ~FlagModule() { _scope.Dispose(); }

        [Command("flag", RunMode = RunMode.Async)]
        [RequireContext(ContextType.DM)]
        public async Task Flag(string flag)
        {
            var challenge = await SharedFlagManager.GetChallByFlag(_context, flag, includeArchived: true);
            var config = await _context.Configuration.AsQueryable().FirstOrDefaultAsync();
            if (challenge == null)
            {
                await ReplyAsync("Your flag is incorrect.");
                if (config != null && config.GuildId != 0 && config.LogsChannelId != 0)
                {
                    var logsChannel = _client.GetGuild(config.GuildId).GetTextChannel(config.LogsChannelId);
                    await logsChannel.SendMessageAsync($"<@{Context.User.Id}> submitted a wrong flag: **{Format.Sanitize(flag).Replace("@", "@\u200B")}**");
                }
                return;
            }

            if (challenge.State == 3) {
                await ReplyAsync("You are trying to submit a flag for an archived challenge.");
                if (config != null && config.GuildId != 0 && config.LogsChannelId != 0) {
                    var logsChannel = _client.GetGuild(config.GuildId).GetTextChannel(config.LogsChannelId);
                    await logsChannel.SendMessageAsync($"<@{Context.User.Id}> submitted a flag for an archived challenge: **{Format.Sanitize(flag).Replace("@", "@\u200B")}**");
                }
                return;
            }

            var user = await UserManager.GetOrCreateUser(_context, Context.User.Id, Context.User.ToString());

            bool isTeam = (user.Team != null);

            if ((!isTeam && user.SolvedChallenges.Contains(challenge)) || (isTeam && user.Team.SolvedChallenges.Contains(challenge)))
            {
                await ReplyAsync("You already solved that challenge!");
                return;
            }

            if (config != null && config.GuildId != 0 && config.TodaysRoleId != 0)
            {
                var lastChall = await _context.Challenges.AsQueryable().OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync(x => x.State == 2);
                if (lastChall == challenge)
                {
                    var guildUser = _client.GetGuild(config.GuildId).GetUser(Context.User.Id);
                    if (guildUser != null)
                    {
                        var role = _client.GetGuild(config.GuildId).GetRole(config.TodaysRoleId);
                        await guildUser.AddRoleAsync(role);
                    }
                }
            }

            if (isTeam)
            {
                user.Team.Score += challenge.Points;
                user.Team.SolvedChallenges.Add(challenge);
                user.Team.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                user.Score += challenge.Points;
                user.SolvedChallenges.Add(challenge);
                user.LastUpdated = DateTime.UtcNow;
            }

            var solve = new Solve
            {
                User = user,
                Team = user.Team,
                Challenge = challenge,
                SolvedAt = DateTime.UtcNow,
                Announced = true
            };

            await _context.Solves.AddAsync(solve);
            await _context.SaveChangesAsync();
            await ReplyAsync($"Congratulations! You solved **{challenge.Title}** challenge!");

            await SolvesManager.AnnounceWebsiteSolves(_client, _context);
            await SolvesManager.AnnounceSolve(_client, _context, challenge, user);
            await LeaderboardManager.UpdateLeaderboard(_client, _context);
            await RolesManager.UpdateRoles(_client, _context);
        }

        [Command("verify")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task Verify(string flag)
        {
            Challenge chall = await SharedFlagManager.GetChallByFlag(_context, flag);
            if (chall == null)
            {
                await ReplyAsync("Your flag is incorrect.");
            }
            else
            {
                await ReplyAsync($"Your flag for **{chall.Title}** is correct.");
            }
        }
    }
}
