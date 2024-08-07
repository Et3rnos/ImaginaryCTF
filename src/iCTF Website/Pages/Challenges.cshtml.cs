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
using AsyncKeyedLock;

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
        private readonly AsyncKeyedLocker<string> _asyncLocker;

        public ChallengesModel(ILogger<ChallengesModel> logger, DatabaseContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AsyncKeyedLocker<string> asyncLocker)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _asyncLocker = asyncLocker;
        }

        public void OnGet() { }

        public async Task OnPostAsync(string flag)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                Error = "You must log in first in order to submit a flag";
                return;
            }

            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config != null && config.IsFinished)
            {
                Error = "The competition is already over";
                return;
            }

            using var asyncLock = await _asyncLocker.LockAsync("flag-submission");

            var appUser = await _userManager.GetUserAsync(User);

            flag = flag?.Trim();
            var challenge = await SharedFlagManager.GetChallByFlag(_context, flag, includeArchived: true);
            if (challenge == null) {
                _logger.LogInformation($"User \"{appUser.UserName}\" submitted a wrong flag ({flag})");
                Error = "Your flag is incorrect!";
                return;
            }

            if (challenge.State == 3) {
                Error = "You are trying to submit a flag for an archived challenge";
                return;
            }

            await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team).LoadAsync();

            if (appUser.User.Team == null) {
                await _context.Entry(appUser.User).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();
            } else {
                await _context.Entry(appUser.User.Team).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();
            }

            if ((appUser.User.Team == null && appUser.User.Solves.Select(x => x.Challenge).Contains(challenge)) || (appUser.User.Team != null && appUser.User.Team.Solves.Select(x => x.Challenge).Contains(challenge)))
            {
                Error = "You already solved this challenge!";
                return;
            }

            var time = DateTime.UtcNow;
            if (time.Hour == 13 && time.Minute == 37)
            {
                _logger.LogInformation($"User \"{appUser.UserName}\" got the \"13:37 Hacker\" role");
                await _userManager.AddToRoleAsync(appUser, "13:37 Hacker");
            }

            if (appUser.User.Team != null) {
                appUser.User.Team.LastUpdated = DateTime.UtcNow;
            } else {
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
