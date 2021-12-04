using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;

namespace iCTF_Shared_Resources.Managers
{
    public class SharedLeaderboardManager
    {
        public static async Task<List<UserTeamUnion>> GetTopUsersAndTeams(DatabaseContext context, int max = 10, bool dynamicScoring = false)
        {
            var users = await context.Users
                .AsExpandable()
                .Where(x => x.Solves.Any() && x.Team == null)
                .Select(x => new UserTeamUnion
                {
                    Id = x.Id,
                    Team = x.Team,
                    DiscordId = x.DiscordId,
                    DiscordUsername = x.DiscordUsername,
                    LastUpdated = x.LastUpdated,
                    Score = dynamicScoring ? x.Solves.Sum(x => DynamicScoringManager.SolvePoints.Invoke(x.Challenge.Solves.Count)) : x.Solves.Sum(x => x.Challenge.Points),
                    WebsiteUser = x.WebsiteUser,
                    IsTeam = false
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.LastUpdated)
                .Take(max)
                .ToListAsync();

            var teams = await context.Teams
                .AsExpandable()
                .Where(x => x.Solves.Any())
                .Select(x => new UserTeamUnion
                {
                    Id = x.Id,
                    LastUpdated = x.LastUpdated,
                    TeamName = x.Name,
                    Score = dynamicScoring ? x.Solves.Sum(x => DynamicScoringManager.SolvePoints.Invoke(x.Challenge.Solves.Count)) : x.Solves.Sum(x => x.Challenge.Points),
                    IsTeam = true
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.LastUpdated)
                .Take(max)
                .ToListAsync();

            return users.Union(teams).OrderByDescending(x => x.Score).ThenBy(x => x.LastUpdated).Take(max).ToList();
        }

        public static async Task<int> GetPosition(DatabaseContext context, int score, DateTime lastUpdated, bool dynamicScoring = false)
        {
            int teamsCount = await context.Teams
                .AsExpandable()
                .Where(x => x.Solves.Any())
                .Where(x =>
                    ((dynamicScoring ? x.Solves.Sum(x => DynamicScoringManager.SolvePoints.Invoke(x.Challenge.Solves.Count)) : x.Solves.Sum(x => x.Challenge.Points)) > score) ||
                    ((dynamicScoring ? x.Solves.Sum(x => DynamicScoringManager.SolvePoints.Invoke(x.Challenge.Solves.Count)) : x.Solves.Sum(x => x.Challenge.Points)) == score && x.LastUpdated < lastUpdated)
                ).CountAsync();
            int playersCount = await context.Users
                .AsExpandable()
                .Where(x => x.Solves.Any())
                .Where(x =>
                    ((dynamicScoring ? x.Solves.Sum(x => DynamicScoringManager.SolvePoints.Invoke(x.Challenge.Solves.Count)) : x.Solves.Sum(x => x.Challenge.Points)) > score ||
                    ((dynamicScoring ? x.Solves.Sum(x => DynamicScoringManager.SolvePoints.Invoke(x.Challenge.Solves.Count)) : x.Solves.Sum(x => x.Challenge.Points)) == score && x.LastUpdated < lastUpdated)) &&
                    x.Team == null
                ).CountAsync();
            return teamsCount + playersCount + 1;
        }
    }
}
