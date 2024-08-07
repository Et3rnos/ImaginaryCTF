﻿using AsyncKeyedLock;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
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
    public class FlagsController : Controller {

        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AsyncKeyedLocker<string> _asyncLocker;

        public FlagsController(DatabaseContext context, UserManager<ApplicationUser> userManager, AsyncKeyedLocker<string> asyncLocker)
        {
            _context = context;
            _userManager = userManager;
            _asyncLocker = asyncLocker;
        }

        public class SubmitData
        {
            [Required]
            public string Flag { get; set; }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAsync(string apiKey, [FromBody] SubmitData data)
        {
            if (!ModelState.IsValid) return BadRequest();

            using var asyncLock = await _asyncLocker.LockAsync("flag-submission");

            string flag = data.Flag.Trim();

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.ApiKey == apiKey);
            if (user == null) return StatusCode(403);

            var config = await _context.Configuration.FirstOrDefaultAsync();
            if (config != null && config.IsFinished)
            {
                return Json(new { Success = false, Error = "The competition is already over" });
            }

            var challenge = await SharedFlagManager.GetChallByFlag(_context, flag, includeArchived: true);
            if (challenge == null)
            {
                return Json(new { Success = false, Error = "Your flag is incorrect" });
            } 
            else if (challenge.State == 3) 
            {
                return Json(new { Success = false, Error = "You are trying to submit a flag for an archived challenge" });
            }

            await _context.Entry(user).Reference(x => x.User).Query().Include(x => x.Team).LoadAsync();
            bool isTeam = (user.User.Team != null);

            if (isTeam)
            {
                await _context.Entry(user.User.Team).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();
            }
            else
            {
                await _context.Entry(user.User).Collection(x => x.Solves).Query().Include(x => x.Challenge).LoadAsync();
            }

            if ((!isTeam && user.User.Solves.Select(x => x.Challenge).Contains(challenge)) || (isTeam && user.User.Team.Solves.Select(x => x.Challenge).Contains(challenge)))
            {
                return Json(new { Success = false, Error = "You already solved that challenge" });
            }

            if (isTeam)
            {
                user.User.Team.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                user.User.LastUpdated = DateTime.UtcNow;
            }

            var solve = new Solve
            {
                User = user.User,
                Team = user.User.Team,
                Challenge = challenge,
                SolvedAt = DateTime.UtcNow,
                Announced = false
            };

            await _context.Solves.AddAsync(solve);
            await _context.SaveChangesAsync();

            return Json(new { 
                Success = true, Challenge = new
                {
                    challenge.Id,
                    challenge.Title
                }
            });
        }
    }
}
