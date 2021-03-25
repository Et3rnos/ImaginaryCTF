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
        public List<User> Users { get; set; }
        public List<ApplicationUser> AppUsers { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _context;

        public LeaderboardModel(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            Users = await SharedLeaderboardManager.GetTopPlayers(_context, 100);
            AppUsers = await _userManager.Users.ToListAsync();
        }
    }
}
