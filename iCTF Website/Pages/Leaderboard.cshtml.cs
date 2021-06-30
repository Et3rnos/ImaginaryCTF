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
using Microsoft.AspNetCore.Identity;

namespace iCTF_Website.Pages
{
    public class LeaderboardModel : PageModel
    {
        public List<LeaderboardItem> Items { get; set; } = new List<LeaderboardItem>();

        private readonly DatabaseContext _context;

        public class LeaderboardItem {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Score { get; set; }
            public DateTime LastUpdated { get; set; }
            public bool IsTeam { get; set; }
        }

        public LeaderboardModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            var users = await SharedLeaderboardManager.GetTopPlayers(_context, int.MaxValue);
            var teams = await SharedLeaderboardManager.GetTopTeams(_context, int.MaxValue);

            foreach (var user in users) {
                Items.Add(new LeaderboardItem {
                    Id = user.Id,
                    Name = user.WebsiteUser?.UserName ?? user.DiscordUsername,
                    Score = user.Score,
                    LastUpdated = user.LastUpdated,
                    IsTeam = false
                });
            }

            foreach (var team in teams) {
                Items.Add(new LeaderboardItem {
                    Id = team.Id,
                    Name = team.Name,
                    Score = team.Score,
                    LastUpdated = team.LastUpdated,
                    IsTeam = true
                });
            }

            Items = Items.OrderByDescending(x => x.Score).ThenBy(x => x.LastUpdated).ToList();
        }
    }
}
