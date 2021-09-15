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

namespace iCTF_Discord_Bot.Logic
{
    public static class UserLogic
    {
        #region Stats
        public static async Task StatsSlashAsync(SocketSlashCommand command, DiscordSocketClient client, DatabaseContext dbContext)
        {
            var user = command.Data.Options?.FirstOrDefault(x => x.Name == "user")?.Value as IUser ?? command.User;
            var player = await GetPlayerFromUserAsync(dbContext, user);
            if (player == null) { await command.RespondAsync("That user is not on the leaderboard yet."); return; }
            var embed = await GetStatsEmbedAsync(client, dbContext, user, player);
            await command.RespondAsync(embed: embed);
        }

        public static async Task StatsCommandAsync(SocketCommandContext context, DiscordSocketClient client, DatabaseContext dbContext, IUser user = null)
        {
            if (user == null) user = context.User;
            var player = await GetPlayerFromUserAsync(dbContext, user);
            if (player == null) { await context.Channel.SendMessageAsync("That user is not on the leaderboard yet."); return; }
            var embed = await GetStatsEmbedAsync(client, dbContext, user, player);
            await context.Channel.SendMessageAsync(embed: embed);
        }

        private static async Task<User> GetPlayerFromUserAsync(DatabaseContext dbContext, IUser user)
        {
            return await dbContext.Users.AsQueryable()
                .Include(x => x.Solves).ThenInclude(x => x.Challenge)
                .Include(x => x.WebsiteUser)
                .Include(x => x.Team).ThenInclude(x => x.Solves).ThenInclude(x => x.Challenge)
                .FirstOrDefaultAsync(x => x.DiscordId == user.Id);
        }

        private static async Task<Embed> GetStatsEmbedAsync(DiscordSocketClient client, DatabaseContext dbContext, IUser user, User player)
        {
            bool isTeam = player.Team != null;

            SharedStatsManager.Stats stats;
            if (isTeam) stats = await SharedStatsManager.GetTeamStats(dbContext, player.Team);
            else stats = await SharedStatsManager.GetStats(dbContext, player);

            var solvedChallengesTitles = stats.SolvedChallenges.Select(x => x.Title).ToList();
            var unsolvedChallengesTitles = stats.UnsolvedChallenges.Select(x => x.Title).ToList();

            var eb = new CustomEmbedBuilder();

            if (isTeam)
            {
                eb.WithTitle($"Stats for {player.Team.Name} (team)");
                eb.AddField("Score", $"{player.Team.Score} ({stats.Position}/{stats.PlayersCount})");
            }
            else
            {
                eb.WithTitle($"Stats for {user.Username}");
                eb.WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());
                eb.AddField("Score", $"{player.Score} ({stats.Position}/{stats.PlayersCount})");
            }

            if (solvedChallengesTitles.Count > 0) eb.AddField("Solved Challenges", string.Join('\n', solvedChallengesTitles), true);
            if (unsolvedChallengesTitles.Count > 0) eb.AddField("Unsolved Challenges", string.Join('\n', unsolvedChallengesTitles), true);

            return eb.Build();
        }
        #endregion
    }
}
