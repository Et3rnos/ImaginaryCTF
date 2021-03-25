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

        public async Task OnPostAsync(string action)
        {
            if (ModelState.IsValid)
            {
                switch (action)
                {
                    case "endround":
                        _context.Solves.RemoveRange(await _context.Solves.ToListAsync());
                        foreach (var user in await _context.Users.ToListAsync())
                        {
                            user.Score = 0;
                        }
                        foreach (var chall in await _context.Challenges.Where(x => x.State == 2).ToListAsync())
                        {
                            chall.State = 3;
                        }
                        await _context.SaveChangesAsync();
                        break;
                }
            }
        }
    }
}
