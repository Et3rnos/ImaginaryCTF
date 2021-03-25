using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCTF_Website.Pages
{
    public class IndexModel : PageModel
    {
        public Challenge Chall { get; set; }
        public Config Config { get; set; }
        public bool Solved { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly DatabaseContext _context;

        public IndexModel(ILogger<IndexModel> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Challenge challenge = await _context.Challenges.Where(x => x.State == 2).OrderByDescending(x => x.ReleaseDate).FirstOrDefaultAsync();
            Config config = await _context.Configuration.FirstOrDefaultAsync();
            Chall = challenge;
            Config = config;
            /*Solve solvedChall = await _context.Solves.Where(x => x.ChallengeId == Chall.Id).FirstOrDefaultAsync();
            if (solvedChall == null)
            {
                Solved = true;
            }
            else
            {
                Solved = false;
            }*/
        }
    }
}
