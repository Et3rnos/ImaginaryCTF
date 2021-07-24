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
using Microsoft.Extensions.Logging;

namespace iCTF_Website.Pages
{
    public class ChallengesModel : PageModel
    {
        public string Error { get; set; }
        public string Success { get; set; }

        private readonly ILogger<ChallengesModel> _logger;
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ChallengesModel(ILogger<ChallengesModel> logger, DatabaseContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet() { }

        public async Task OnPostAsync(string flag)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                Error = "You must log in first in order to submit a flag";
                return;
            }

            var appUser = await _userManager.GetUserAsync(User);
            await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team).LoadAsync();

            var challenge = await SharedFlagManager.GetChallByFlag(_context, flag, includeArchived: true);
            if (challenge == null) {
                _logger.LogInformation($"User \"{appUser.UserName}\" submitted a wrong flag ({flag})");
                Error = "Your flag is incorrect";
                return;
            }

            if (challenge.State == 3) {
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
                Error = "You already solved that challenge";
                return;
            }

            var time = DateTime.UtcNow;
            if (time.Hour == 13 && time.Minute == 37)
            {
                _logger.LogInformation($"User \"{appUser.UserName}\" got the \"13:37 Hacker\" role");
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

            Success = $"Congratulations! You solved {challenge.Title} challenge!";
        }
    }
}
