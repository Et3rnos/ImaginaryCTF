using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Managers
{
    public class SharedLeaderboardManager
    {
        public static async Task<List<UserTeamUnion>> GetTopUsersAndTeams(DatabaseContext context, int max = 10, bool dynamicScoring = false)
        {
            var users = await context.Users
                .Where(x => x.Solves.Sum(x => x.Challenge.Points) > 0 && x.Team == null)
                .Select(x => new UserTeamUnion { Id = x.Id, Team = x.Team, DiscordId = x.DiscordId, DiscordUsername = x.DiscordUsername, LastUpdated = x.LastUpdated, Score = dynamicScoring ? x.Solves.Sum(x => Math.Max(Convert.ToInt32((100 - 500) / 5000f * Convert.ToInt32(Math.Pow(x.Challenge.Solves.Count, 2)) + 500), 100)) : x.Solves.Sum(x => x.Challenge.Points), SolvesCount = x.Solves.Count, WebsiteUser = x.WebsiteUser, IsTeam = false })
                .ToListAsync();
            var teams = await context.Teams
                .Where(x => x.Solves.Sum(x => x.Challenge.Points) > 0)
                .Select(x => new UserTeamUnion { Id = x.Id, LastUpdated = x.LastUpdated, TeamName = x.Name, TeamCode = x.Code, Score = dynamicScoring ? x.Solves.Sum(x => Math.Max(Convert.ToInt32((100 - 500) / 5000f * Convert.ToInt32(Math.Pow(x.Challenge.Solves.Count, 2)) + 500), 100)) : x.Solves.Sum(x => x.Challenge.Points), SolvesCount = x.Solves.Count, MembersCount = x.Members.Count, IsTeam = true })
                .ToListAsync();
            var players = users.Union(teams).OrderByDescending(x => x.Score).ThenBy(x => x.LastUpdated).Take(max).ToList();
            return players;
        }

        public static async Task<List<User>> GetTopPlayers(DatabaseContext context, int max = 10)
        {
            return await context.Users
                .Include(x => x.WebsiteUser)
                .Include(x => x.Solves)
                .ThenInclude(x => x.Challenge)
                .Where(x => x.Solves.Sum(x => x.Challenge.Points) > 0 && x.Team == null)
                .OrderByDescending(x => x.Solves.Sum(x => x.Challenge.Points))
                .ThenBy(x => x.LastUpdated)
                .Take(max)
                .ToListAsync();
        }

        public static async Task<List<Team>> GetTopTeams(DatabaseContext context, int max = 10) {
            return await context.Teams
                .Include(x => x.Solves)
                .ThenInclude(x => x.Challenge)
                .Where(x => x.Solves.Sum(x => x.Challenge.Points) > 0)
                .OrderByDescending(x => x.Solves.Sum(x => x.Challenge.Points))
                .ThenBy(x => x.LastUpdated)
                .Take(max)
                .ToListAsync();
        }

        /* Inneficient functions - DO NOT USE
        public static async Task<int> GetPlayerPosition(DatabaseContext context, User user)
        {
            int score = user.Solves.Sum(x => x.Challenge.Points);
            return await context.Users.Where(x => ((x.Solves.Sum(x => x.Challenge.Points) > score) || (x.Solves.Sum(x => x.Challenge.Points) == score && x.LastUpdated < user.LastUpdated)) && x.Team == null).CountAsync() + 1;
        }

        public static async Task<int> GetTeamPosition(DatabaseContext context, Team team) {
            int score = team.Solves.Sum(x => x.Challenge.Points);
            return await context.Teams.Where(x => (x.Solves.Sum(x => x.Challenge.Points) > score) || (x.Solves.Sum(x => x.Challenge.Points) == score && x.LastUpdated < team.LastUpdated)).CountAsync() + 1;
        }
        */

        public static async Task<int> GetPosition(DatabaseContext context, Team team) {
            int score = team.Solves.Sum(x => x.Challenge.Points);
            return await GetPosition(context, score, team.LastUpdated);
        }

        public static async Task<int> GetPosition(DatabaseContext context, User user) {
            int score = user.Solves.Sum(x => x.Challenge.Points);
            return await GetPosition(context, score, user.LastUpdated);
        }

        private static async Task<int> GetPosition(DatabaseContext context, int score, DateTime lastUpdated) {
            int teamsCount = await context.Teams
                .Where(x => 
                    (x.Solves.Sum(x => x.Challenge.Points) > score) || 
                    (x.Solves.Sum(x => x.Challenge.Points) == score && x.LastUpdated < lastUpdated)
                ).CountAsync();
            int playersCount = await context.Users
                .Where(x =>
                    ((x.Solves.Sum(x => x.Challenge.Points) > score) ||
                    (x.Solves.Sum(x => x.Challenge.Points) == score && x.LastUpdated < lastUpdated)) &&
                    x.Team == null
                ).CountAsync();
            return teamsCount + playersCount + 1;
        }
    }
}
