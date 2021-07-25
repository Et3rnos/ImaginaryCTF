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
using Newtonsoft.Json;

namespace iCTF_Website.Pages
{
    public class ArchivedChallengesModel : PageModel
    {
        public int Index { get; set; }
        public string Round { get; set; }
        public List<int> Rounds { get; set; }
        public List<Challenge> Challenges { get; set; }
        public List<string> Categories { get; set; }

        private readonly DatabaseContext _context;

        public ArchivedChallengesModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync([Range(0,1000)] int round = 0)
        {
            if (!ModelState.IsValid) return NotFound();

            var now = DateTime.UtcNow;
            var date = new DateTime(2020, 1, 1);

            int offset = now.Month - date.Month + (now.Year - date.Year) * 12;

            Rounds = await _context.Challenges.Where(x => x.State == 3).Select(x => (x.ReleaseDate.Month - date.Month) + (x.ReleaseDate.Year - date.Year) * 12).Distinct().OrderBy(x => x).ToListAsync();

            round--;

            if (!Rounds.Contains(round))
                if (Rounds.Any())
                    round = Rounds.Last();
                else
                    round = offset;

            Index = round;

            int pageMonthOffset = round % 12;
            int pageYearOffset = round / 12;

            int month = date.Month + pageMonthOffset;
            int year = date.Year + pageYearOffset;

            if (month < 1)
            {
                month += 12;
                year -= 1;
            }

            Round = $"{new DateTime(2020, month, 1).ToString("MMMM", CultureInfo.InvariantCulture)} {year}";

            Challenges = await _context.Challenges.Where(x => x.State == 3 && x.ReleaseDate.Month == month && x.ReleaseDate.Year == year).OrderBy(x => x.ReleaseDate).ToListAsync();

            Categories = Challenges.Select(x => x.Category).Distinct().ToList();

            return Page();
        }
    }
}
