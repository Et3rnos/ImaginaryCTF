using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using iCTF_Shared_Resources.Managers;

namespace iCTF_Website.Pages
{
    public class ChallengesModel : PageModel
    {
        public string Error { get; set; }
        public string Success { get; set; }
        public List<ChallengeInfo> Challenges { get; set; } = new List<ChallengeInfo>();
        public List<ChallengeInfo> SolvedChallenges { get; set; } = new List<ChallengeInfo>();

        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ChallengesModel(DatabaseContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task OnGetAsync()
        {
            if (_signInManager.IsSignedIn(User)) {
                var appUser = await _userManager.GetUserAsync(User);
                await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team).LoadAsync();
                await PopulateChallenges(appUser.User);
            } else { 
                Challenges = await _context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, UserSolvesCount = x.UserSolves.Count, TeamSolvesCount = x.TeamSolves.Count }).ToListAsync();
            }
        }

        public async Task OnPostAsync(string flag)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                Challenges = await _context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).Select(x => new ChallengeInfo { Challenge = x, UserSolvesCount = x.UserSolves.Count, TeamSolvesCount = x.TeamSolves.Count }).ToListAsync();
                Error = "You must log in first in order to submit a flag";
                return;
            }

            var appUser = await _userManager.GetUserAsync(User);
            await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team).LoadAsync();

            var challenge = await SharedFlagManager.GetChallByFlag(_context, flag, includeArchived: true);
            if (challenge == null) {
                await PopulateChallenges(appUser.User);
                Error = "Your flag is incorrect";
                return;
            }

            if (challenge.State == 3) {
                await PopulateChallenges(appUser.User);
                Error = "You are trying to submit a flag for an archived challenge";
                return;
            }

            if (appUser.User.Team == null) {
                await _context.Entry(appUser.User).Collection(x => x.SolvedChallenges).LoadAsync();
            } else {
                await _context.Entry(appUser.User.Team).Collection(x => x.SolvedChallenges).LoadAsync();
            }

            if ((appUser.User.Team == null && appUser.User.SolvedChallenges.Contains(challenge)) || (appUser.User.Team != null && appUser.User.Team.SolvedChallenges.Contains(challenge)))
            {
                await PopulateChallenges(appUser.User);
                Error = "You already solved that challenge";
                return;
            }

            var time = DateTime.UtcNow;
            if (time.Hour == 13 && time.Minute == 37)
            {
                await _userManager.AddToRoleAsync(appUser, "13:37 Hacker");
            }

            if (appUser.User.Team != null) {
                appUser.User.Team.Score += challenge.Points;
                appUser.User.Team.SolvedChallenges.Add(challenge);
                appUser.User.Team.LastUpdated = DateTime.UtcNow;
            } else {
                appUser.User.Score += challenge.Points;
                appUser.User.SolvedChallenges.Add(challenge);
                appUser.User.LastUpdated = DateTime.UtcNow;
            }

            var solve = new Solve
            {
                User = appUser.User,
                Team = appUser.User.Team,
                Challenge = challenge,
                SolvedAt = DateTime.UtcNow,
                Announced = false
            };

            await _context.Solves.AddAsync(solve);
            await _context.SaveChangesAsync();

            await PopulateChallenges(appUser.User);

            Success = $"Congratulations! You solved {challenge.Title} challenge!";
        }

        public async Task PopulateChallenges(User user) {
            SharedStatsManager.FullStats stats;
            if (user.Team == null) {
                stats = await SharedStatsManager.GetFullStats(_context, user);
            } else {
                stats = await SharedStatsManager.GetFullTeamStats(_context, user.Team);
            }
            Challenges = stats.UnsolvedChallenges;
            SolvedChallenges = stats.SolvedChallenges;
        }
    }
}
