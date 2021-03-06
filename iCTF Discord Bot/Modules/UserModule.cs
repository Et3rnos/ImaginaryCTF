using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace iCTF_Discord_Bot.Modules
{
    [Name("user")]
    public class UserModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly DatabaseContext _context;
        private readonly IServiceScope _scope;

        private UserModule(DiscordSocketClient client, CommandService commands, IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _commands = commands;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
        }

        ~UserModule() { _scope.Dispose(); }

        [Command("stats")]
        [Summary("Prints the player statistics")]
        public async Task Stats([Name("user")] IUser dUser = null)
        {
            if (dUser == null) {
                dUser = Context.User;
            }

            var user = await _context.Users.AsQueryable().Include(x => x.Solves).ThenInclude(x => x.Challenge).Include(x => x.WebsiteUser).Include(x => x.Team).ThenInclude(x => x.Solves).ThenInclude(x => x.Challenge).FirstOrDefaultAsync(x => x.DiscordId == dUser.Id);

            if (user == null) {
                await ReplyAsync("That user is not on the leaderboard yet");
                return;
            }

            bool isTeam = (user.Team != null);

            SharedStatsManager.Stats stats;
            if (isTeam)
            {
                stats = await SharedStatsManager.GetTeamStats(_context, user.Team);
            }
            else
            {
                stats = await SharedStatsManager.GetStats(_context, user);
            }

            var solvedChallengesTitles = stats.SolvedChallenges.Select(x => x.Title).ToList();
            var unsolvedChallengesTitles = stats.UnsolvedChallenges.Select(x => x.Title).ToList();

            CustomEmbedBuilder eb = new CustomEmbedBuilder();

            if (isTeam)
            {
                eb.WithTitle($"Stats for {user.Team.Name} (team)");
                eb.AddField("Score", $"{user.Team.Score} ({stats.Position}/{stats.PlayersCount})");
            }
            else
            {
                eb.WithTitle($"Stats for {_client.GetUser(user.DiscordId).Username}");
                eb.WithThumbnailUrl(dUser.GetAvatarUrl() ?? dUser.GetDefaultAvatarUrl());
                eb.AddField("Score", $"{user.Score} ({stats.Position}/{stats.PlayersCount})");
            }

            if (solvedChallengesTitles.Count > 0)
            {
                eb.AddField("Solved Challenges", string.Join('\n', solvedChallengesTitles), true);
            }
            if (unsolvedChallengesTitles.Count > 0)
            {
                eb.AddField("Unsolved Challenges", string.Join('\n', unsolvedChallengesTitles), true);
            }

            await ReplyAsync(embed: eb.Build());
        }
    }
}
