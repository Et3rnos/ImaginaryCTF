using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using iCTF_Website.Helpers;

namespace iCTF_Website.Areas.Account.Pages
{
    [Authorize]
    public class TeamModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Team Team { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }

        [BindProperty]
        public CreateJoinModel CreateJoin { get; set; }

        public class CreateJoinModel {
            public CreateModel CreateModel { get; set; }
            public JoinModel JoinModel { get; set; }
        }

        public class CreateModel {
            [Required]
            [StringLength(20)]
            public string TeamName { get; set; }
        }

        public class JoinModel {
            [Required]
            public string TeamName {get; set; }
            [Required]
            public string TeamCode {get; set; }
        }

        public TeamModel(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGetAsync() {
            var appUser = await _userManager.GetUserAsync(User);
            await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team).LoadAsync();
            Team = appUser.User.Team;
        }

        public async Task<IActionResult> OnPostCreateAsync() {
            if (!ModelState.IsValid || CreateJoin.CreateModel == null) {
                return Page();
            }

            var appUser = await _userManager.GetUserAsync(User);
            await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team).Include(x => x.Solves).LoadAsync();
            Team = appUser.User.Team;
            if (Team != null) {
                return Forbid();
            }

            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Name == CreateJoin.CreateModel.TeamName);
            if (team != null) {
                Error = "That team already exist.";
                return Page();
            }

            appUser.User.Team = new Team {
                Name = CreateJoin.CreateModel.TeamName,
                Code = RandomHelper.GenerateRandomString(),
            };
            appUser.User.Solves.Clear();
            Team = appUser.User.Team;
            Success = $"You have successfully created {CreateJoin.CreateModel.TeamName} team.";

            await _context.SaveChangesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostJoinAsync() {
            if (!ModelState.IsValid || CreateJoin.JoinModel == null) {
                return Page();
            }

            var appUser = await _userManager.GetUserAsync(User);
            await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team).Include(x => x.Solves).LoadAsync();
            Team = appUser.User.Team;
            if (Team != null) {
                return Forbid();
            }

            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Name == CreateJoin.JoinModel.TeamName);
            if (team == null) {
                Error = "That team does not exist.";
                return Page();
            }

            if (team.Code == CreateJoin.JoinModel.TeamCode) {
                Team = team;
                appUser.User.Team = team;
                appUser.User.Solves.Clear();
                Success = $"You have successfully joined {team.Name} team.";
            } else {
                Error = "Invalid team code.";
            }

            await _context.SaveChangesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostLeaveAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            await _context.Entry(appUser).Reference(x => x.User).Query().Include(x => x.Team.Solves).Include(x => x.Team).ThenInclude(x => x.Members).Include(x => x.Solves).ThenInclude(x => x.Challenge).LoadAsync();
            var team = appUser.User.Team;
            if (team == null) return Forbid();

            if (team.Members.Count == 1)
                _context.Teams.Remove(team);
            else
            {
                appUser.User.Team = null;
                team.Solves.RemoveAll(x => appUser.User.Solves.Select(x => x.ChallengeId).Contains(x.ChallengeId));
            }

            appUser.User.Solves.Clear();

            await _context.SaveChangesAsync();

            Success = "You left your team";

            return Page();
        }
    }
}
