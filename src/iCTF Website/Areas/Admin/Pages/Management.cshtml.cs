using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace iCTF_Website.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class ManagementModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public ManagementModel(DatabaseContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Success { get; set; }
        public string Error { get; set; }

        public List<string> Roles { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [Display(Name = "Role Name")]
            public string RoleName { get; set; }
        }

        public async Task OnGetAsync()
        {
            Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid) {
                Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
                return Page();
            }

            var user = await _userManager.FindByNameAsync(Input.Username);

            if (user == null)
            {
                Error = "That user does not exist";
                Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
                return Page();
            }

            await _userManager.AddToRoleAsync(user, Input.RoleName);

            Success = "That role was added to the user you specified";

            Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync()
        {
            if (!ModelState.IsValid) {
                Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
                return Page();
            }

            var user = await _userManager.FindByNameAsync(Input.Username);

            if (user == null)
            {
                Error = "That user does not exist";
                Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
                return Page();
            }

            await _userManager.RemoveFromRoleAsync(user, Input.RoleName);

            Success = "That role was removed from the user you specified";

            Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            return Page();
        }
    }
}
