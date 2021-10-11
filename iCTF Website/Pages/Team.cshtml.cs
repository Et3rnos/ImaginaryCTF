using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using iCTF_Shared_Resources.Managers;
using static iCTF_Shared_Resources.Managers.SharedStatsManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace iCTF_Website.Pages
{
    public class TeamModel : PageModel
    {
        public Team Team { get; set; }
        public Stats TeamStats { get; set; }
        public DateTime FirstChallengeReleaseDate { get; set; }
        
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public TeamModel(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0) {
                Team = new Team {
                    Name = "Roo Gang",
                    Members = new List<User> {
                        new User { Id = 0, WebsiteUser = new ApplicationUser { UserName = "rooYay" } },
                        new User { Id = 0, WebsiteUser = new ApplicationUser { UserName = "rooNobooli" } },
                        new User { Id = 0, WebsiteUser = new ApplicationUser { UserName = "rooMad" } }
                    }
                };
                TeamStats = new Stats { 
                    SolvedChallenges = new List<ChallengeInfo> { new ChallengeInfo { Challenge = new Challenge { Title = "Hack ImaginaryCTF", Points = 1337 } } },
                    PlayersCount = 611,
                    Position = -1,
                    Score = 1337
                };
                return Page();
            }

            Team = await _context.Teams.Include(x => x.Members).ThenInclude(x => x.WebsiteUser).Include(x => x.Solves).ThenInclude(x => x.Challenge).FirstOrDefaultAsync(x => x.Id == id);
            if (Team == null) return NotFound();


            TeamStats = await GetTeamStats(_context, Team, _configuration.GetValue<bool>("DynamicScoring"));
            if (TeamStats.SolvedChallenges.Any())
            {
                var challs = TeamStats.SolvedChallenges.Union(TeamStats.UnsolvedChallenges);
                FirstChallengeReleaseDate = challs.Min(x => x.Challenge.ReleaseDate);
            }

            return Page();
        }
    }
}
