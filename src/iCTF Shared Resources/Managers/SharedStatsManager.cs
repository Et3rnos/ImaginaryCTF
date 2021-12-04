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
            var solvedChallenges = await context.Solves.Where(x => x.User == user).Select(x => new ChallengeInfo { Challenge = x.Challenge, SolvesCount = x.Challenge.Solves.Count }).ToListAsync();

            int teamsCount = await context.Teams.Where(x => x.Solves.Any()).CountAsync();
            int playersCount = await context.Users.Where(x => x.Solves.Any() && x.Team == null).CountAsync();

            var challengesInfo = await context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, SolvesCount = x.Solves.Count }).ToListAsync();

            int score;
            if (dynamicScoring)
                score = solvedChallenges.Sum(x => DynamicScoringManager.GetPointsFromSolvesCount(x.SolvesCount));
            else
                score = solvedChallenges.Sum(x => x.Challenge.Points);

            int position = await SharedLeaderboardManager.GetPosition(context, score, user.LastUpdated, dynamicScoring);

            var stats = new Stats {
                Position = position,
                PlayersCount = playersCount + teamsCount,
                Score = score
            };

            foreach (var challengeInfo in challengesInfo) {
                if (solvedChallenges.Select(x => x.Challenge).Contains(challengeInfo.Challenge)) {
                    stats.SolvedChallenges.Add(challengeInfo);
                } else {
                    stats.UnsolvedChallenges.Add(challengeInfo);
                }
            }

            return stats;
        }

        public static async Task<Stats> GetTeamStats(DatabaseContext context, Team team, bool dynamicScoring = false) {
            var solvedChallenges = await context.Solves.Where(x => x.Team == team).Select(x => new ChallengeInfo { Challenge = x.Challenge, SolvesCount = x.Challenge.Solves.Count }).ToListAsync();

            int teamsCount = await context.Teams.Where(x => x.Solves.Any()).CountAsync();
            int playersCount = await context.Users.Where(x => x.Solves.Any() && x.Team == null).CountAsync();

            var challengesInfo = await context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, SolvesCount = x.Solves.Count }).ToListAsync();

            int score;
            if (dynamicScoring)
                score = solvedChallenges.Sum(x => DynamicScoringManager.GetPointsFromSolvesCount(x.SolvesCount));
            else
                score = solvedChallenges.Sum(x => x.Challenge.Points);

            int position = await SharedLeaderboardManager.GetPosition(context, score, team.LastUpdated, dynamicScoring);

            var stats = new Stats {
                Position = position,
                PlayersCount = teamsCount + playersCount,
                Score = score
            };

            foreach(var challengeInfo in challengesInfo) {
                if (solvedChallenges.Select(x => x.Challenge).Contains(challengeInfo.Challenge)) {
                    stats.SolvedChallenges.Add(challengeInfo);
                } else {
                    stats.UnsolvedChallenges.Add(challengeInfo);
                }
            }

            return stats;
        }
    }
}
