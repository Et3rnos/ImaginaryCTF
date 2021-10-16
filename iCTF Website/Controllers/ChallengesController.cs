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

        private bool dynamicScoring;

        public ChallengesController(DatabaseContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            dynamicScoring = _configuration.GetValue<bool>("DynamicScoring");
        }

        [HttpPost("submit")]
        [RequireRoles("Administrator")]
        public async Task<IActionResult> SubmitAsync([FromBody] SubmitModel input)
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
                Title = input.Title.Trim(),
                Category = input.Category.Trim(),
                Description = input.Description.Trim(),
                Attachments = input.Attachments.Trim(),
                Flag = input.Flag.Trim(),
                Author = input.Author.Trim(),
                Points = input.Points,
                Writeup = input.Writeup.Trim(),
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
            var challenges = await _context.Challenges.Where(x => x.State == 0).ToListAsync();
            var apiChallenges = new List<UnapprovedChallenge>();
            foreach (var chall in challenges)
            {
                apiChallenges.Add(new UnapprovedChallenge
                {
                    Id = chall.Id,
                    Title = chall.Title,
                    Category = chall.Category,
                    Description = chall.Description,
                    Attachments = chall.Attachments,
                    Writeup = chall.Writeup,
                    Flag = chall.Flag,
                    Author = chall.Author,
                    Points = chall.Points
                });
            }
            return Json(apiChallenges);
        }

        [HttpGet("approved")]
        [RequireRoles("Administrator")]
        public async Task<IActionResult> ApprovedAsync()
        {
            var challenges = await _context.Challenges.Where(x => x.State == 1).ToListAsync();
            var apiChallenges = new List<ApprovedChallenge>();
            foreach (var chall in challenges)
            {
                apiChallenges.Add(new ApprovedChallenge
                {
                    Id = chall.Id,
                    Title = chall.Title,
                    Category = chall.Category,
                    Description = chall.Description,
                    Attachments = chall.Attachments,
                    Writeup = chall.Writeup,
                    Flag = chall.Flag,
                    Author = chall.Author,
                    Points = chall.Points,
                    Priority = chall.Priority
                });
            }
            return Json(apiChallenges);
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
                    Points = dynamicScoring ? DynamicScoringManager.SolvePoints.Invoke(x.Solves.Count) : x.Points,
                    release_date = x.ReleaseDate,

                })
                .ToListAsync();
            return Json(challenges);
        }

        [HttpGet("archived")]
        public async Task<IActionResult> ArchivedAsync() {
            var challenges = await _context.Challenges.Where(x => x.State == 3).OrderByDescending(x => x.ReleaseDate).ToListAsync();
            var apiChallenges = new List<ArchivedChallenge>();
            foreach (var chall in challenges) {
                apiChallenges.Add(new ArchivedChallenge {
                    Id = chall.Id,
                    Title = chall.Title,
                    Category = chall.Category,
                    Description = chall.Description,
                    Attachments = chall.Attachments,
                    Writeup = chall.Writeup,
                    Flag = chall.Flag,
                    Author = chall.Author,
                    Points = chall.Points,
                    ReleaseDate = chall.ReleaseDate
                });
            }
            return Json(apiChallenges);
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

        class UnapprovedChallenge
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string Attachments { get; set; }
            public string Writeup { get; set; }
            public string Flag { get; set; }
            public string Author { get; set; }
            public int Points { get; set; }
        }

        class ApprovedChallenge
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string Attachments { get; set; }
            public string Writeup { get; set; }
            public string Flag { get; set; }
            public string Author { get; set; }
            public int Points { get; set; }
            public int Priority { get; set; }
        }

        class ArchivedChallenge {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string Attachments { get; set; }
            public string Writeup { get; set; }
            public string Flag { get; set; }
            public string Author { get; set; }
            public int Points { get; set; }
            [JsonPropertyName("release_date")]
            public DateTime ReleaseDate { get; set; }
        }
    }
}
