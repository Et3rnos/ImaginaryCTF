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
    public class DangerZoneModel : PageModel
    {
        private readonly DatabaseContext _context;

        public DangerZoneModel(DatabaseContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            
        }

        public async Task OnPostReleaseAsync()
        {
            foreach (var chall in await _context.Challenges.Where(x => x.State == 1).OrderBy(x => x.Priority).ToListAsync())
            {
                chall.State = 2;
                chall.ReleaseDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        public async Task OnPostArchiveAsync()
        {            
            foreach (var chall in await _context.Challenges.Where(x => x.State == 2).ToListAsync())
            {
                chall.State = 3;
            }
            await _context.SaveChangesAsync();
        }

        public async Task OnPostResetAsync()
        {
            _context.Solves.RemoveRange(await _context.Solves.ToListAsync());
            foreach (var user in await _context.Users.Include(x => x.Solves).ToListAsync())
            {
                user.Solves.Clear();
            }
            foreach (var team in await _context.Teams.Include(x => x.Solves).ToListAsync())
            {
                team.Solves.Clear();
            }
            await _context.SaveChangesAsync();
        }

        public async Task OnPostEndAsync()
        {
            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config == null)
            {
                await _context.Configuration.AddAsync(new Config { IsFinished = true });
            }
            else
            {
                config.IsFinished = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}
