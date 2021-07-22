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

namespace iCTF_Website.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class ChallengeSubmissionModel : PageModel
    {
        private readonly DatabaseContext _context;
        
        public ChallengeSubmissionModel(DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Title { get; set; }

            [Required]
            public string Category { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public string Attachments { get; set; }

            public string Writeup { get; set; }

            [Required]
            public string Flag { get; set; }

            [Required]
            public string Author { get; set; }

            [Required]
            public int Points { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                int max;
                try
                {
                    max = await _context.Challenges.MaxAsync(x => x.Priority);
                }
                catch
                {
                    max = 0;
                }
                Challenge challenge = new Challenge()
                {
                    Title = Input.Title.Trim(),
                    Category = Input.Category.Trim(),
                    Description = Input.Description.Trim(),
                    Attachments = Input.Attachments.Trim(),
                    Flag = Input.Flag.Trim(),
                    Author = Input.Author.Trim(),
                    Points = Input.Points,
                    Writeup = Input.Writeup.Trim(),
                    State = 0,
                    Priority = max + 1
                };
                await _context.Challenges.AddAsync(challenge);
                await _context.SaveChangesAsync();
                return RedirectToPage("./NonReleasedChallenges");
            }

            return Page();
        }
    }
}
