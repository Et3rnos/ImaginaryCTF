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
        public static async Task<List<UserTeamUnion>> GetTopUsersAndTeams(DatabaseContext context, int max = 10)
        {
            var users = await context.Users.Where(x => x.Score > 0 && x.Team == null)
                .Select(x => new UserTeamUnion { Id = x.Id, Team = x.Team, DiscordId = x.DiscordId, DiscordUsername = x.DiscordUsername, LastUpdated = x.LastUpdated, Score = x.Score, SolvedChallengesCount = x.SolvedChallenges.Count, WebsiteUser = x.WebsiteUser, IsTeam = false })
                .ToListAsync();
            var teams = await context.Teams.Where(x => x.Score > 0)
                .Select(x => new UserTeamUnion { Id = x.Id, LastUpdated = x.LastUpdated, TeamName = x.Name, TeamCode = x.Code, Score = x.Score, SolvedChallengesCount = x.SolvedChallenges.Count, MembersCount = x.Members.Count, IsTeam = true })
                .ToListAsync();
            var players = users.Union(teams).OrderByDescending(x => x.Score).ThenBy(x => x.LastUpdated).Take(max).ToList();
            return players;
        }

        public static async Task<List<User>> GetTopPlayers(DatabaseContext context, int max = 10)
        {
            return await context.Users.Include(x => x.WebsiteUser).Where(x => x.Score > 0 && x.Team == null).OrderByDescending(x => x.Score).ThenBy(x => x.LastUpdated).Take(max).ToListAsync();
        }

        public static async Task<List<Team>> GetTopTeams(DatabaseContext context, int max = 10) {
            return await context.Teams.Where(x => x.Score > 0).OrderByDescending(x => x.Score).ThenBy(x => x.LastUpdated).Take(max).ToListAsync();
        }

        public static async Task<int> GetPlayerPosition(DatabaseContext context, User user)
        {
            return await context.Users.Where(x => ((x.Score > user.Score) || (x.Score == user.Score && x.LastUpdated < user.LastUpdated)) && x.Team == null).CountAsync() + 1;
        }

        public static async Task<int> GetTeamPosition(DatabaseContext context, Team team) {
            return await context.Teams.Where(x => (x.Score > team.Score) || (x.Score == team.Score && x.LastUpdated < team.LastUpdated)).CountAsync() + 1;
        }

        public static async Task<int> GetPosition(DatabaseContext context, Team team) {
            return await GetPosition(context, team.Score, team.LastUpdated);
        }

        public static async Task<int> GetPosition(DatabaseContext context, User user) {
            return await GetPosition(context, user.Score, user.LastUpdated);
        }

        private static async Task<int> GetPosition(DatabaseContext context, int score, DateTime lastUpdated) {
            int teamsCount = await context.Teams.Where(x => (x.Score > score) || (x.Score == score && x.LastUpdated < lastUpdated)).CountAsync();
            int playersCount = await context.Users.Where(x => ((x.Score > score) || (x.Score == score && x.LastUpdated < lastUpdated)) && x.Team == null).CountAsync();
            return teamsCount + playersCount + 1;
        }
    }
}
