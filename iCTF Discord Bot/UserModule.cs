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

namespace iCTF_Discord_Bot
{
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
        [Summary("Prints the player statistics.")]
        public async Task Stats(IUser dUser = null)
        {
            if (dUser == null)
            {
                dUser = Context.User;
            }

            User user = (await _context.Users.AsAsyncEnumerable().Where(x => x.DiscordId == dUser.Id).ToListAsync()).FirstOrDefault();

            if (user == null)
            {
                await ReplyAsync("That user is not on the leaderboard yet");
                return;
            }

            SharedStatsManager.Stats stats = await SharedStatsManager.GetStats(_context, user);

            List<string> solvedChallengesTitles = stats.SolvedChallenges.Select(x => x.Title).ToList();
            List<string> unsolvedChallengesTitles = stats.UnsolvedChallenges.Select(x => x.Title).ToList();

            CustomEmbedBuilder eb = new CustomEmbedBuilder();
            eb.WithTitle($"Stats for {_client.GetUser(user.DiscordId).Username}");
            eb.WithThumbnailUrl(dUser.GetAvatarUrl() ?? dUser.GetDefaultAvatarUrl());
            eb.AddField("Score", $"{user.Score} ({stats.Position}/{stats.PlayersCount})");

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
