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
    public class LeaderboardController : Controller {

        private readonly DatabaseContext _context;

        public LeaderboardController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("ctftime")]
        [RequireRoles("Administrator")]
        public async Task<IActionResult> CtftimeAsync()
        {
            var top = await SharedLeaderboardManager.GetTopUsersAndTeams(_context, int.MaxValue);
            var teams = new List<dynamic>();

            for (int i = 0; i < top.Count; i++)
            {
                teams.Add(new { 
                    Pos = i + 1,
                    Team = top[i].IsTeam ? top[i].TeamName : (top[i].WebsiteUser?.UserName ?? top[i].DiscordUsername), 
                    Score = top[i].Score
                });
            }
            return Json(new { standings = teams });
        }
    }
}
