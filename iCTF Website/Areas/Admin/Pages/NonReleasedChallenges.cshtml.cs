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
    public class NonReleasedChallengesModel : PageModel
    {

        public List<Challenge> NonApprovedChallenges { get; set; }
        public List<Challenge> ApprovedChallenges { get; set; }

        private readonly DatabaseContext _context;

        public NonReleasedChallengesModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            await GetChalls();
        }

        public async Task OnPostAsync(string action, int id)
        {
            if (ModelState.IsValid)
            {
                List<Challenge> challs = await _context.Challenges.Where(x => x.Id == id).ToListAsync();
                Challenge chall = challs.FirstOrDefault();
                if (chall != null)
                {
                    switch (action)
                    {
                        case "approve":
                            if (chall.State == 0)
                            {
                                chall.State = 1;
                            }
                            break;
                        case "unapprove":
                            if (chall.State == 1)
                            {
                                chall.State = 0;
                            }
                            break;
                        case "delete":
                            if (chall.State == 0 || chall.State == 1)
                            {
                                _context.Challenges.Remove(chall);
                            }
                            break;
                        case "up":
                            if (chall.State == 1)
                            {
                                Challenge upChall = await _context.Challenges.Where(x => x.State == 1 && x.Priority < chall.Priority).OrderByDescending(x => x.Priority).FirstOrDefaultAsync();
                                if (upChall == null)
                                {
                                    break;
                                }
                                int tmp = upChall.Priority;
                                upChall.Priority = chall.Priority;
                                chall.Priority = tmp;
                            }
                            break;
                        case "down":
                            if (chall.State == 1)
                            {
                                Challenge downChall = await _context.Challenges.Where(x => x.State == 1 && x.Priority > chall.Priority).OrderBy(x => x.Priority).FirstOrDefaultAsync();
                                if (downChall == null)
                                {
                                    break;
                                }
                                int tmp = downChall.Priority;
                                downChall.Priority = chall.Priority;
                                chall.Priority = tmp;
                            }
                            break;
                    }
                    await _context.SaveChangesAsync();
                }
            }
            await GetChalls();
        }

        private async Task GetChalls()
        {
            List<Challenge> challenges = await _context.Challenges.Where(x => x.State != 2).OrderBy(x => x.Priority).ToListAsync();
            NonApprovedChallenges = challenges.Where(x => x.State == 0).ToList();
            ApprovedChallenges = challenges.Where(x => x.State == 1).ToList();
        }
    }
}
