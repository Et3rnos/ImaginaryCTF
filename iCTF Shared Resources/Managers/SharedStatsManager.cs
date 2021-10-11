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

        public class Stats {
            public List<ChallengeInfo> SolvedChallenges { get; set; } = new List<ChallengeInfo>();
            public List<ChallengeInfo> UnsolvedChallenges { get; set; } = new List<ChallengeInfo>();
            public int Position { get; set; }
            public int PlayersCount { get; set; }
            public int Score { get; set;  }
        }

        public static async Task<Stats> GetStats(DatabaseContext context, User user, bool dynamicScoring = false) {
            await context.Entry(user).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();

            int teamsCount = await context.Teams.Where(x => x.Solves.Any()).CountAsync();
            int playersCount = await context.Users.Where(x => x.Solves.Any() && x.Team == null).CountAsync();
            int position = await SharedLeaderboardManager.GetPosition(context, user);

            var challengesInfo = await context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, SolvesCount = x.Solves.Count }).ToListAsync();
           

            var solvedChallenges = user.Solves.Select(x => x.Challenge).ToList();

            int score;
            if (dynamicScoring)
                score = solvedChallenges.Sum(x => DynamicScoringManager.GetPointsFromSolvesCount(x.Solves.Count));
            else
                score = solvedChallenges.Sum(x => x.Points);

            var stats = new Stats {
                Position = position,
                PlayersCount = playersCount + teamsCount,
                Score = score
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

        public static async Task<Stats> GetTeamStats(DatabaseContext context, Team team, bool dynamicScoring = false) {
            await context.Entry(team).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();

            int teamsCount = await context.Teams.Where(x => x.Solves.Any()).CountAsync();
            int playersCount = await context.Users.Where(x => x.Solves.Any() && x.Team == null).CountAsync();
            int position = await SharedLeaderboardManager.GetPosition(context, team);

            var challengesInfo = await context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, SolvesCount = x.Solves.Count }).ToListAsync();

            var solvedChallenges = team.Solves.Select(x => x.Challenge).ToList();

            int score;
            if (dynamicScoring)
                score = solvedChallenges.Sum(x => DynamicScoringManager.GetPointsFromSolvesCount(x.Solves.Count));
            else
                score = solvedChallenges.Sum(x => x.Points);

            var stats = new Stats {
                Position = position,
                PlayersCount = teamsCount + playersCount,
                Score = score
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
