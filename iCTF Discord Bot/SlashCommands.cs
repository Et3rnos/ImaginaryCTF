using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;
using Discord;

namespace iCTF_Discord_Bot
{
    public class SlashCommands
    {
        private readonly SocketSlashCommand _command;
        private readonly IServiceScope _scope;
        private readonly DiscordSocketClient _client;
        private readonly DatabaseContext _context;

        public SlashCommands(SocketSlashCommand command, IServiceScopeFactory scopeFactory)
        {
            _command = command;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
            _client = _scope.ServiceProvider.GetService<DiscordSocketClient>();
        }

        ~SlashCommands() { _scope.Dispose(); }

        public async Task StatsAsync(IUser dUser)
        {
            if (dUser == null) dUser = _command.User;

            var user = await _context.Users.AsQueryable().Include(x => x.Solves).ThenInclude(x => x.Challenge).Include(x => x.WebsiteUser).Include(x => x.Team).ThenInclude(x => x.Solves).ThenInclude(x => x.Challenge).FirstOrDefaultAsync(x => x.DiscordId == dUser.Id);

            if (user == null)
            {
                await _command.RespondAsync("That user is not on the leaderboard yet");
                return;
            }

            bool isTeam = (user.Team != null);

            SharedStatsManager.Stats stats;
            if (isTeam) stats = await SharedStatsManager.GetTeamStats(_context, user.Team);
            else stats = await SharedStatsManager.GetStats(_context, user);

            var solvedChallengesTitles = stats.SolvedChallenges.Select(x => x.Title).ToList();
            var unsolvedChallengesTitles = stats.UnsolvedChallenges.Select(x => x.Title).ToList();

            var eb = new CustomEmbedBuilder();

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

            if (solvedChallengesTitles.Count > 0) eb.AddField("Solved Challenges", string.Join('\n', solvedChallengesTitles), true);
            if (unsolvedChallengesTitles.Count > 0) eb.AddField("Unsolved Challenges", string.Join('\n', unsolvedChallengesTitles), true);

            await _command.RespondAsync(embed: eb.Build());
        }

        public async Task Leaderboard()
        {
            var users = await SharedLeaderboardManager.GetTopUsersAndTeams(_context, 20);

            var eb = new CustomEmbedBuilder();
            eb.WithTitle("Leaderboard");

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].IsTeam)
                {
                    eb.Description += $"\n**{i + 1}. {users[i].TeamName}** (team) - {users[i].Score} points";
                }
                else
                {
                    eb.Description += $"\n**{i + 1}. {users[i].WebsiteUser?.UserName ?? users[i].DiscordUsername}** - {users[i].Score} points";
                }
            }

            var builder = new ComponentBuilder().WithButton("Full Leaderboard", style: ButtonStyle.Link, url: "https://imaginaryctf.org/leaderboard");
            await _command.RespondAsync(embed: eb.Build(), component: builder.Build(), ephemeral: true);
        }
    }
}
