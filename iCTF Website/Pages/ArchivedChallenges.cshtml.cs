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

namespace iCTF_Website.Pages
{
    public class ArchivedChallengesModel : PageModel
    {

        public List<Challenge> Challenges { get; set; }

        private readonly DatabaseContext _context;

        public ArchivedChallengesModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            List<Challenge> challenges = await _context.Challenges.Where(x => x.State == 3).OrderBy(x => x.ReleaseDate).ToListAsync();
            Challenges = challenges;
        }
    }
}
