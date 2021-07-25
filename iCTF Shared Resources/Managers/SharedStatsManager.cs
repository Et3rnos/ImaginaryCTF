using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Managers
{
    public class SharedStatsManager
    {
        public class Stats
        {
            public List<Challenge> SolvedChallenges { get; set; } = new List<Challenge> ();
            public List<Challenge> UnsolvedChallenges { get; set; } = new List<Challenge> ();
            public int Position { get; set; }
            public int PlayersCount { get; set; }
        }

        public static async Task<Stats> GetStats(DatabaseContext context, User user)
        {
            int teamsCount = await context.Teams.Where(x => x.Score > 0).CountAsync();
            int playersCount = await context.Users.Where(x => x.Score > 0 && x.Team == null).CountAsync();
            int position = await SharedLeaderboardManager.GetPosition(context, user);

            var challenges = await context.Challenges.Where(x => x.State == 2).ToListAsync();
            await context.Entry(user).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();

            var solvedChallenges = user.Solves.Select(x => x.Challenge).ToList();

            var unsolvedChallenges = challenges.Except(solvedChallenges).ToList();

            var stats = new Stats {
                Position = position,
                PlayersCount = playersCount + teamsCount,
                SolvedChallenges = solvedChallenges,
                UnsolvedChallenges = unsolvedChallenges
            };

            return stats;
        }

        public static async Task<Stats> GetTeamStats(DatabaseContext context, Team team) {
            int teamsCount = await context.Teams.Where(x => x.Score > 0).CountAsync();
            int playersCount = await context.Users.Where(x => x.Score > 0 && x.Team == null).CountAsync();
            int position = await SharedLeaderboardManager.GetPosition(context, team);

            var challenges = await context.Challenges.Where(x => x.State == 2).ToListAsync();
            await context.Entry(team).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();

            var solvedChallenges = team.Solves.Select(x => x.Challenge).ToList();

            var unsolvedChallenges = challenges.Except(solvedChallenges).ToList();

            Stats stats = new Stats {
                Position = position,
                PlayersCount = teamsCount + playersCount,
                SolvedChallenges = solvedChallenges,
                UnsolvedChallenges = unsolvedChallenges
            };

            return stats;
        }

        public class FullStats {
            public List<ChallengeInfo> SolvedChallenges { get; set; } = new List<ChallengeInfo>();
            public List<ChallengeInfo> UnsolvedChallenges { get; set; } = new List<ChallengeInfo>();
            public int Position { get; set; }
            public int PlayersCount { get; set; }
        }

        public static async Task<FullStats> GetFullStats(DatabaseContext context, User user) {
            int teamsCount = await context.Teams.Where(x => x.Score > 0).CountAsync();
            int playersCount = await context.Users.Where(x => x.Score > 0 && x.Team == null).CountAsync();
            int position = await SharedLeaderboardManager.GetPosition(context, user);

            var challengesInfo = await context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, SolvesCount = x.Solves.Count }).ToListAsync();
            await context.Entry(user).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();

            var solvedChallenges = user.Solves.Select(x => x.Challenge).ToList();

            var stats = new FullStats {
                Position = position,
                PlayersCount = playersCount + teamsCount
            };

            foreach (var challengeInfo in challengesInfo) {
                if (solvedChallenges.Contains(challengeInfo.Challenge)) {
                    stats.SolvedChallenges.Add(challengeInfo);
                } else {
                    stats.UnsolvedChallenges.Add(challengeInfo);
                }
            }

            return stats;
        }

        public static async Task<FullStats> GetFullTeamStats(DatabaseContext context, Team team) {
            int teamsCount = await context.Teams.Where(x => x.Score > 0).CountAsync();
            int playersCount = await context.Users.Where(x => x.Score > 0 && x.Team == null).CountAsync();
            int position = await SharedLeaderboardManager.GetPosition(context, team);

            var challengesInfo = await context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, SolvesCount = x.Solves.Count }).ToListAsync();
            await context.Entry(team).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();

            var solvedChallenges = team.Solves.Select(x => x.Challenge).ToList();

            var stats = new FullStats {
                Position = position,
                PlayersCount = teamsCount + playersCount
            };

            foreach(var challengeInfo in challengesInfo) {
                if (solvedChallenges.Contains(challengeInfo.Challenge)) {
                    stats.SolvedChallenges.Add(challengeInfo);
                } else {
                    stats.UnsolvedChallenges.Add(challengeInfo);
                }
            }

            return stats;
        }
    }
}
