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
using Microsoft.Extensions.Configuration;

namespace iCTF_Website.Pages
{
    public class LeaderboardModel : PageModel
    {
        public List<UserTeamUnion> Items { get; set; } = new List<UserTeamUnion>();

        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public LeaderboardModel(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            Items = await SharedLeaderboardManager.GetTopUsersAndTeams(_context, int.MaxValue, _configuration.GetValue<bool>("DynamicScoring"));
        }
    }
}
