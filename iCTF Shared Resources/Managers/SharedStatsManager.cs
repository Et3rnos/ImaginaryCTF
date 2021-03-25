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
            public List<Challenge> SolvedChallenges { get; set; }
            public List<Challenge> UnsolvedChallenges { get; set; }
            public int Position { get; set; }
            public int PlayersCount { get; set; }
        }

        public static async Task<Stats> GetStats(DatabaseContext context, User user)
        {
            int playersCount = await context.Users.Where(x => x.Score > 0).CountAsync();
            int position = await SharedLeaderboardManager.GetPlayerPosition(context, user);

            List<Challenge> challenges = await context.Challenges.Where(x => x.State == 2).ToListAsync();
            List<Solve> solves = await context.Solves.Where(x => x.UserId == user.Id).ToListAsync();

            List<int> solvedChallengesIds = solves.Select(x => x.ChallengeId).ToList();

            List<Challenge> solvedChallenges = challenges.Where(x => solvedChallengesIds.Contains(x.Id)).ToList();
            List<Challenge> unsolvedChallenges = challenges.Where(x => !solvedChallengesIds.Contains(x.Id)).ToList();

            Stats stats = new Stats()
            {
                Position = position,
                PlayersCount = playersCount,
                SolvedChallenges = solvedChallenges,
                UnsolvedChallenges = unsolvedChallenges
            };

            return stats;
        }
    }
}
