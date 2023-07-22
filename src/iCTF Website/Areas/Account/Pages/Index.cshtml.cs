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
using iCTF_Website.Helpers;
using Serilog;

namespace iCTF_Website.Areas.Account.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser ApplicationUser { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            ApplicationUser = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(ApplicationUser.VerificationToken))
            {
                ApplicationUser.VerificationToken = RandomHelper.GenerateRandomString();
                var result = await _userManager.UpdateAsync(ApplicationUser);
            }
        }
    }
}
