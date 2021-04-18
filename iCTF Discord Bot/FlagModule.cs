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

        [Command("flag")]
        [RequireContext(ContextType.DM)]
        public async Task Flag(string flag)
        {
            Challenge challenge = await SharedFlagManager.GetChallByFlag(_context, flag);
            Config config = await _context.Configuration.FirstOrDefaultAsync();
            if (challenge == null)
            {
                await ReplyAsync("Your flag is incorrect.");
                if (config != null && config.GuildId != 0 && config.LogsChannelId != 0)
                {
                    var logsChannel = _client.GetGuild(config.GuildId).GetTextChannel(config.LogsChannelId);
                    await logsChannel.SendMessageAsync($"<@{Context.User.Id}> submitted a wrong flag: **{Format.Sanitize(flag)}**");
                }
                return;
            }

            User user = await UserManager.GetOrCreateUser(_context, Context.User.Id, Context.User.ToString());

            Solve alreadySolved = await _context.Solves.AsAsyncEnumerable()
                .Where(x => x.UserId == user.Id && x.ChallengeId == challenge.Id).FirstOrDefaultAsync();
            if (alreadySolved != null)
            {
                await ReplyAsync("You already solved that challenge!");
                return;
            }

            if (config != null && config.GuildId != 0 && config.TodaysRoleId != 0)
            {
                var lastChall = await _context.Challenges.AsAsyncEnumerable().Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync();
                if (lastChall.Id == challenge.Id)
                {
                    var guildUser = _client.GetGuild(config.GuildId).GetUser(Context.User.Id);
                    if (guildUser != null)
                    {
                        var role = _client.GetGuild(config.GuildId).GetRole(config.TodaysRoleId);
                        await guildUser.AddRoleAsync(role);
                    }
                }
            }

            user.Score += challenge.Points;
            user.DiscordUsername = Context.User.ToString();
            user.LastUpdated = DateTime.UtcNow;

            Solve solve = new Solve
            {
                UserId = user.Id,
                Username = Context.User.ToString(),
                ChallengeId = challenge.Id,
                ChallengeTitle = challenge.Title,
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
