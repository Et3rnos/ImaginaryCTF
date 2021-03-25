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
        public List<Challenge> Challenges { get; set; } = new List<Challenge>();
        public List<Challenge> SolvedChallenges { get; set; } = new List<Challenge>();

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
            if (_signInManager.IsSignedIn(User))
            {
                var appUser = await _userManager.GetUserAsync(User);
                var user = await _context.Users.Where(x => x.Id == appUser.UserId).FirstOrDefaultAsync();
                var stats = await SharedStatsManager.GetStats(_context, user);
                Challenges = stats.UnsolvedChallenges;
                SolvedChallenges = stats.SolvedChallenges;
            }
            else 
            { 
                Challenges = await _context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).ToListAsync();
            }
        }

        public async Task OnPostAsync(string flag)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                Challenges = await _context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).ToListAsync();
                Error = "You must log in first in order to submit a flag";
                return;
            }

            var appUser = await _userManager.GetUserAsync(User);
            var user = await _context.Users.Where(x => x.Id == appUser.UserId).FirstOrDefaultAsync();

            SharedStatsManager.Stats stats;

            var challenge = await SharedFlagManager.GetChallByFlag(_context, flag);
            if (challenge == null)
            {
                stats = await SharedStatsManager.GetStats(_context, user);
                Challenges = stats.UnsolvedChallenges;
                SolvedChallenges = stats.SolvedChallenges;
                Error = "Your flag is incorrect";
                return;
            }

            var alreadySolved = await _context.Solves.Where(x => x.UserId == user.Id && x.ChallengeId == challenge.Id).FirstOrDefaultAsync();
            if (alreadySolved != null)
            {
                stats = await SharedStatsManager.GetStats(_context, user);
                Challenges = stats.UnsolvedChallenges;
                SolvedChallenges = stats.SolvedChallenges;
                Error = "You already solved that challenge";
                return;
            }

            user.Score += challenge.Points;
            user.LastUpdated = DateTime.UtcNow;

            var solve = new Solve
            {
                UserId = user.Id,
                Username = appUser.UserName,
                ChallengeId = challenge.Id,
                ChallengeTitle = challenge.Title,
                SolvedAt = DateTime.UtcNow,
                Announced = false
            };

            await _context.Solves.AddAsync(solve);
            await _context.SaveChangesAsync();

            stats = await SharedStatsManager.GetStats(_context, user);
            Challenges = stats.UnsolvedChallenges;
            SolvedChallenges = stats.SolvedChallenges;

            Success = $"Congratulations! You solved {challenge.Title} challenge!";
        }
    }
}
