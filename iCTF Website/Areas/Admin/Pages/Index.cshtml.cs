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
using Microsoft.EntityFrameworkCore;

namespace iCTF_Website.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        public int WebsiteUsersCount { get; set; }
        public int UsersCount { get; set; }
        public int TeamsCount { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _context;

        public IndexModel(UserManager<ApplicationUser> userManager, DatabaseContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            WebsiteUsersCount = await _userManager.Users.CountAsync();
            UsersCount = await _context.Users.CountAsync();
            TeamsCount = await _context.Teams.CountAsync();
        }
    }
}
