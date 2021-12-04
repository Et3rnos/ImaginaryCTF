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
    public class ReleasedChallengesModel : PageModel
    {

        public List<Challenge> Challenges { get; set; }

        private readonly DatabaseContext _context;

        public ReleasedChallengesModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Challenges = await _context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).ToListAsync();
        }
    }
}
