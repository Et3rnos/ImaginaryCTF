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
using Microsoft.Extensions.Configuration;

namespace iCTF_Website.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class SolvesController : Controller
    {

        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly bool _dynamicScoring;

        public SolvesController(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _dynamicScoring = configuration.GetValue<bool>("DynamicScoring");
        }

        [HttpGet("last/{limit}")]
        public async Task<IActionResult> LastAsync([Range(1, 100)] int limit)
        {
            var solves = await _context.Solves
                .OrderByDescending(x => x.SolvedAt)
                .Take(limit)
                .Select(x => new {
                    x.Id,
                    User = x.User != null ? new {
                        x.User.Id,
                        discord_id = x.User.DiscordId,
                        discord_username = x.User.DiscordUsername,
                        WebsiteUser = x.User.WebsiteUser != null ? new {
                            x.User.WebsiteUser.UserName
                        } : null
                    } : null,
                    Team = x.Team != null ? new {
                        x.Team.Id,
                        x.Team.Name,
                    } : null,
                    Challenge = x.Challenge != null ? new {
                        x.Challenge.Id,
                        x.Challenge.Title
                    } : null,
                    solved_at = x.SolvedAt,
                    x.Announced
                })
                .ToListAsync();
            return Json(solves);
        }

        [HttpGet("byuserid/{id}")]
        public async Task<IActionResult> ByUserId(int id)
        {
            var solves = await _context.Solves
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.SolvedAt)
                .Select(x => new {
                    x.Id,
                    User = x.User != null ? new
                    {
                        x.User.Id,
                        discord_id = x.User.DiscordId,
                        discord_username = x.User.DiscordUsername,
                        WebsiteUser = x.User.WebsiteUser != null ? new
                        {
                            x.User.WebsiteUser.UserName
                        } : null
                    } : null,
                    Team = x.Team != null ? new
                    {
                        x.Team.Id,
                        x.Team.Name,
                    } : null,
                    Challenge = x.Challenge != null ? new
                    {
                        x.Challenge.Id,
                        x.Challenge.Title
                    } : null,
                    solved_at = x.SolvedAt,
                    x.Announced
                })
                .ToListAsync();
            return Json(solves);
        }

        [HttpGet("byteamid/{id}")]
        public async Task<IActionResult> ByTeamId(int id)
        {
            var solves = await _context.Solves
                .Where(x => x.User.Team.Id == id)
                .OrderByDescending(x => x.SolvedAt)
                .Select(x => new {
                    x.Id,
                    User = x.User != null ? new
                    {
                        x.User.Id,
                        discord_id = x.User.DiscordId,
                        discord_username = x.User.DiscordUsername,
                        WebsiteUser = x.User.WebsiteUser != null ? new
                        {
                            x.User.WebsiteUser.UserName
                        } : null
                    } : null,
                    Team = x.Team != null ? new
                    {
                        x.Team.Id,
                        x.Team.Name,
                    } : null,
                    Challenge = x.Challenge != null ? new
                    {
                        x.Challenge.Id,
                        x.Challenge.Title
                    } : null,
                    solved_at = x.SolvedAt,
                    x.Announced
                })
                .ToListAsync();
            return Json(solves);
        }

        [HttpGet("bydiscordid/{id}")]
        public async Task<IActionResult> ByDiscordId(ulong id)
        {
            var solves = await _context.Solves
                .Where(x => id != 0 && x.User.DiscordId == id)
                .OrderByDescending(x => x.SolvedAt)
                .Select(x => new {
                    x.Id,
                    User = x.User != null ? new
                    {
                        x.User.Id,
                        discord_id = x.User.DiscordId,
                        discord_username = x.User.DiscordUsername,
                        WebsiteUser = x.User.WebsiteUser != null ? new
                        {
                            x.User.WebsiteUser.UserName
                        } : null
                    } : null,
                    Team = x.Team != null ? new
                    {
                        x.Team.Id,
                        x.Team.Name,
                    } : null,
                    Challenge = x.Challenge != null ? new
                    {
                        x.Challenge.Id,
                        x.Challenge.Title
                    } : null,
                    solved_at = x.SolvedAt,
                    x.Announced
                })
                .ToListAsync();
            return Json(solves);
        }
    }
}
