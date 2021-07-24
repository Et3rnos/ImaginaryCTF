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
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace iCTF_Website.Pages
{
    public class ArchivedChallengesModel : PageModel
    {
        public int Page { get; set; }
        public string Round { get; set; }
        public List<int> Rounds { get; set; }
        public List<Challenge> Challenges { get; set; }

        private readonly DatabaseContext _context;

        public ArchivedChallengesModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync([Range(0,1000)] int round = 1)
        {
            if (!ModelState.IsValid) return NotFound();

            Page = round;

            int pageMonthOffset = round % 12;
            int pageYearOffset = round / 12;

            var date = DateTime.UtcNow;

            int month = date.Month - pageMonthOffset;
            int year = date.Year - pageYearOffset;

            if (month < 1)
            {
                month += 12;
                year -= 1;
            }

            Rounds = await _context.Challenges.Where(x => x.State == 3).Select(x => (date.Month - x.ReleaseDate.Month) + (date.Year - x.ReleaseDate.Year) * 12).Distinct().ToListAsync();
            Rounds.Sort();

            Round = $"{new DateTime(2000, month, 1).ToString("MMMM", CultureInfo.InvariantCulture)} {year}";

            Challenges = await _context.Challenges.Where(x => x.State == 3 && x.ReleaseDate.Month == month && x.ReleaseDate.Year == year).OrderBy(x => x.ReleaseDate).ToListAsync();

            return Page();
        }
    }
}
