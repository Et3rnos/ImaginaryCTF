using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using iCTF_Website.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace iCTF_Website.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class SolvesController : Controller {

        private readonly DatabaseContext _context;

        public SolvesController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("last/{limit}")]
        public async Task<IActionResult> LastAsync([Range(1,100)] int limit) {
            var solves = await _context.Solves.Include(x => x.User).ThenInclude(x => x.WebsiteUser).Include(x => x.Team).Include(x => x.Challenge).OrderByDescending(x => x.SolvedAt).Take(limit).ToListAsync();
            var apiSolves = solves.Select(x => new ApiSolve
            {
                Id = x.Id,
                User = x.User != null ? new ApiUser
                {
                    Id = x.User.Id,
                    DiscordId = x.User.DiscordId,
                    DiscordUsername = x.User.DiscordUsername,
                    Score = x.User.Score,
                    WebsiteUser = x.User.WebsiteUser != null ? new ApiWebsiteUser
                    {
                        Username = x.User.WebsiteUser.UserName
                    } : null,
                    
                } : null,
                Team = x.Team != null ? new ApiTeam
                {
                    Id = x.Team.Id,
                    Name = x.Team.Name,
                    Score = x.Team.Score
                } : null,
                Challenge = x.Challenge != null ? new ApiChallenge
                {
                    Id = x.Challenge.Id,
                    Title = x.Challenge.Title
                } : null,
                SolvedAt = x.SolvedAt,
                Announced = x.Announced

            }).ToList();
            return Json(apiSolves);
        }

        [HttpGet("byuserid/{id}")]
        public async Task<IActionResult> ByUserId(int id)
        {
            var solves = await _context.Solves.Include(x => x.User).ThenInclude(x => x.WebsiteUser).Include(x => x.Team).Include(x => x.Challenge).OrderByDescending(x => x.SolvedAt).Where(x => x.UserId == id).ToListAsync();
            var apiSolves = solves.Select(x => new ApiSolve
            {
                Id = x.Id,
                User = x.User != null ? new ApiUser
                {
                    Id = x.User.Id,
                    DiscordId = x.User.DiscordId,
                    DiscordUsername = x.User.DiscordUsername,
                    Score = x.User.Score,
                    WebsiteUser = x.User.WebsiteUser != null ? new ApiWebsiteUser
                    {
                        Username = x.User.WebsiteUser.UserName
                    } : null,

                } : null,
                Team = x.Team != null ? new ApiTeam
                {
                    Id = x.Team.Id,
                    Name = x.Team.Name,
                    Score = x.Team.Score
                } : null,
                Challenge = x.Challenge != null ? new ApiChallenge
                {
                    Id = x.Challenge.Id,
                    Title = x.Challenge.Title
                } : null,
                SolvedAt = x.SolvedAt,
                Announced = x.Announced

            }).ToList();
            return Json(apiSolves);
        }

        [HttpGet("byteamid/{id}")]
        public async Task<IActionResult> ByTeamId(int id)
        {
            var solves = await _context.Solves.Include(x => x.User).ThenInclude(x => x.WebsiteUser).Include(x => x.Team).Include(x => x.Challenge).OrderByDescending(x => x.SolvedAt).Where(x => x.User.Team.Id == id).ToListAsync();
            var apiSolves = solves.Select(x => new ApiSolve
            {
                Id = x.Id,
                User = x.User != null ? new ApiUser
                {
                    Id = x.User.Id,
                    DiscordId = x.User.DiscordId,
                    DiscordUsername = x.User.DiscordUsername,
                    Score = x.User.Score,
                    WebsiteUser = x.User.WebsiteUser != null ? new ApiWebsiteUser
                    {
                        Username = x.User.WebsiteUser.UserName
                    } : null,

                } : null,
                Team = x.Team != null ? new ApiTeam
                {
                    Id = x.Team.Id,
                    Name = x.Team.Name,
                    Score = x.Team.Score
                } : null,
                Challenge = x.Challenge != null ? new ApiChallenge
                {
                    Id = x.Challenge.Id,
                    Title = x.Challenge.Title
                } : null,
                SolvedAt = x.SolvedAt,
                Announced = x.Announced

            }).ToList();
            return Json(apiSolves);
        }

        [HttpGet("bydiscordid/{id}")]
        public async Task<IActionResult> ByDiscordId(ulong id)
        {
            var solves = await _context.Solves.Include(x => x.User).ThenInclude(x => x.WebsiteUser).Include(x => x.Team).Include(x => x.Challenge).OrderByDescending(x => x.SolvedAt).Where(x => x.User.DiscordId == id).ToListAsync();
            var apiSolves = solves.Select(x => new ApiSolve
            {
                Id = x.Id,
                User = x.User != null ? new ApiUser
                {
                    Id = x.User.Id,
                    DiscordId = x.User.DiscordId,
                    DiscordUsername = x.User.DiscordUsername,
                    Score = x.User.Score,
                    WebsiteUser = x.User.WebsiteUser != null ? new ApiWebsiteUser
                    {
                        Username = x.User.WebsiteUser.UserName
                    } : null,

                } : null,
                Team = x.Team != null ? new ApiTeam
                {
                    Id = x.Team.Id,
                    Name = x.Team.Name,
                    Score = x.Team.Score
                } : null,
                Challenge = x.Challenge != null ? new ApiChallenge
                {
                    Id = x.Challenge.Id,
                    Title = x.Challenge.Title
                } : null,
                SolvedAt = x.SolvedAt,
                Announced = x.Announced

            }).ToList();
            return Json(apiSolves);
        }

        class ApiSolve
        {
            public int Id { get; set; }
            public ApiUser User { get; set; }
            public ApiTeam Team { get; set; }
            public ApiChallenge Challenge { get; set; }
            public DateTime SolvedAt { get; set; }
            public bool Announced { get; set; }
        }

        class ApiUser
        {
            public int Id { get; set; }
            public ulong DiscordId { get; set; }
            public string DiscordUsername { get; set; }
            public int Score { get; set; }
            public ApiWebsiteUser WebsiteUser {get; set;}
        }

        class ApiWebsiteUser
        {
            public string Username { get; set; }
        }

        class ApiTeam
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Score { get; set; }
        }

        class ApiChallenge
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}
