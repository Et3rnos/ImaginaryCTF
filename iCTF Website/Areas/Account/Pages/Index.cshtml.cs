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

namespace iCTF_Website.Areas.Account.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public int UserId { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            UserId = user.UserId;
        }
    }
}
