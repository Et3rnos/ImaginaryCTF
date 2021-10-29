using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
using iCTF_Shared_Resources.Models;
using iCTF_Website.Attributes;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace iCTF_Website.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class ChallengesController : Controller {

        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        private readonly bool _dynamicScoring;

        public ChallengesController(DatabaseContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _dynamicScoring = _configuration.GetValue<bool>("DynamicScoring");
        }

        [HttpPost("submit")]
        [RequireRoles("Administrator")]
        public async Task<IActionResult> SubmitAsync([FromBody] SubmitModel input)
        {
            int max;
            try { max = await _context.Challenges.MaxAsync(x => x.Priority); }
            catch { max = 0; }

            var challenge = new Challenge()
            {
                Title = input.Title.Trim(),
                Category = input.Category.Trim(),
                Description = input.Description.Trim(),
                Attachments = input.Attachments.Trim(),
                Flag = input.Flag.Trim(),
                Author = input.Author.Trim(),
                Points = input.Points,
                Writeup = input.Writeup?.Trim(),
                State = 0,
                Priority = max + 1
            };
            await _context.Challenges.AddAsync(challenge);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet("unapproved")]
        [RequireRoles("Administrator")]
        public async Task<IActionResult> UnapprovedAsync()
        {
            var challenges = await _context.Challenges
                .Where(x => x.State == 0)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Category,
                    x.Description,
                    x.Attachments,
                    x.Writeup,
                    x.Flag,
                    x.Author,
                    x.Points
                })
                .ToListAsync();
            return Json(challenges);
        }

        [HttpGet("approved")]
        [RequireRoles("Administrator")]
        public async Task<IActionResult> ApprovedAsync()
        {
            var challenges = await _context.Challenges
                .Where(x => x.State == 1)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Category,
                    x.Description,
                    x.Attachments,
                    x.Writeup,
                    x.Flag,
                    x.Author,
                    x.Points,
                    x.Priority
                })
                .ToListAsync();
            return Json(challenges);
        }

        [HttpGet("released")]
        public async Task<IActionResult> ReleasedAsync() {
            var challenges = await _context.Challenges
                .AsExpandable()
                .Where(x => x.State == 2)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Category,
                    x.Description,
                    x.Attachments,
                    x.Author,
                    Points = _dynamicScoring ? DynamicScoringManager.SolvePoints.Invoke(x.Solves.Count) : x.Points,
                    solves_count = x.Solves.Count,
                    release_date = x.ReleaseDate,

                })
                .ToListAsync();
            return Json(challenges);
        }

        [HttpGet("archived")]
        public async Task<IActionResult> ArchivedAsync() {
            var challenges = await _context.Challenges
                .Where(x => x.State == 3)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Category,
                    x.Description,
                    x.Attachments,
                    x.Writeup,
                    x.Flag,
                    x.Author,
                    x.Points,
                    release_date = x.ReleaseDate
                })
                .ToListAsync();
            return Json(challenges);
        }

        public class SubmitModel
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
    }
}
