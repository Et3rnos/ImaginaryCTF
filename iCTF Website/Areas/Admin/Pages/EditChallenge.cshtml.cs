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
    public class EditChallengeModel : PageModel
    {
        private readonly DatabaseContext _context;
        
        public EditChallengeModel(DatabaseContext context)
        {
            _context = context;
        }

        public Challenge Chall { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int id) {
            Chall = await _context.Challenges.FirstOrDefaultAsync(x => x.Id == id && (x.State == 0 || x.State == 1 || x.State == 2));
            if (Chall == null) {
                return NotFound();
            } else {
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Chall = await _context.Challenges.FirstOrDefaultAsync(x => x.Id == id && (x.State == 0 || x.State == 1 || x.State == 2));
            if (Chall == null) {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Chall.Title = Input.Title;
                Chall.Category = Input.Category;
                Chall.Description = Input.Description;
                Chall.Attachments = Input.Attachments;
                Chall.Writeup = Input.Writeup;
                Chall.Author = Input.Author;
                Chall.Flag = Input.Flag;
                Chall.Points = Input.Points;
                
                await _context.SaveChangesAsync();
                return RedirectToPage("./NonReleasedChallenges");
            }

            return Page();
        }
    }
}
