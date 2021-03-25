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

namespace iCTF_Website.Pages
{
    public class UserModel : PageModel
    {
        public string Name { get; set; }
        public ApplicationUser AppUser { get; set; }
        public User Player { get; set; }
        public Stats PlayerStats { get; set; }

        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserModel(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGetAsync(int id)
        {
            Player = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            AppUser = await _userManager.Users.Where(x => x.UserId == id).FirstOrDefaultAsync();
            PlayerStats = await GetStats(_context, Player);

            Name = Player.DiscordUsername;
            if (AppUser != null)
            {
                Name = AppUser.UserName;
            }
        }
    }
}
