using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace iCTF_Website.Pages
{
    public class SolvesModel : PageModel
    {
        public List<Solve> Solves { get; set; }

        private readonly DatabaseContext _context;

        public SolvesModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Solves = await _context.Solves.Include(x => x.User.WebsiteUser).Include(x => x.Challenge).Include(x => x.Team).OrderByDescending(x => x.SolvedAt).Take(50).ToListAsync();
        }
    }
}
