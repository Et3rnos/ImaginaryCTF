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
using System.ComponentModel.DataAnnotations;

namespace iCTF_Website.Areas.Account.Pages
{
    [Authorize]
    public class ApiModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public string ApiKey { get; set; }
        public List<string> Roles { get; set; }
        public List<Endpoint> Endpoints { get; set; }

        public ApiModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Roles = (await _userManager.GetRolesAsync(user)).ToList();
            ApiKey = user.ApiKey;

            Endpoints = new List<Endpoint>
            {
                new Endpoint { 
                    Name = "Released Challenges List",
                    Path = "/api/challenges/released",
                    Method = "GET",
                    RequiresApiKey = false
                },
                new Endpoint {
                    Name = "Archived Challenges List",
                    Path = "/api/challenges/archived",
                    Method = "GET",
                    RequiresApiKey = false
                },
                new Endpoint {
                    Name = "Unapproved Challenges List",
                    Path = "/api/challenges/unapproved",
                    Method = "GET",
                    RequiresApiKey = true,
                    AllowedRoles = new List<string> { "Administrator" },
                },
                new Endpoint {
                    Name = "Approved Challenges List",
                    Path = "/api/challenges/approved",
                    Method = "GET",
                    RequiresApiKey = true,
                    AllowedRoles = new List<string> { "Administrator" },
                },
                new Endpoint {
                    Name = "Flag Submission",
                    Path = "/api/flags/submit",
                    Method = "POST",
                    RequiresApiKey = true,
                    Parameters = new List<string> { "flag" }
                },
                new Endpoint {
                    Name = "Challenge Submission",
                    Path = "/api/challenges/submit",
                    Method = "POST",
                    RequiresApiKey = true,
                    AllowedRoles = new List<string> { "Administrator" },
                    Parameters = new List<string> { "title", "category", "description", "attachments", "writeup", "flag", "author", "points" }
                },
                new Endpoint {
                    Name = "Last Solves",
                    Path = "/api/solves/last/{limit}",
                    Method = "GET",
                    RequiresApiKey = false
                },
                new Endpoint {
                    Name = "Solves By User Id",
                    Path = "/api/solves/byuserid/{id}",
                    Method = "GET",
                    RequiresApiKey = false
                },
                new Endpoint {
                    Name = "Solves By Discord Id",
                    Path = "/api/solves/bydiscordid/{id}",
                    Method = "GET",
                    RequiresApiKey = false
                },
                new Endpoint {
                    Name = "Solves By Team Id",
                    Path = "/api/solves/byteamid/{id}",
                    Method = "GET",
                    RequiresApiKey = false
                },
                new Endpoint {
                    Name = "CTFTime Leaderboard",
                    Path = "/api/leaderboard/ctftime",
                    Method = "GET",
                    RequiresApiKey = true,
                    AllowedRoles = new List<string> { "Administrator" }
                },
            };
        }

        public class Endpoint
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Method { get; set; }
            public List<string> AllowedRoles { get; set; } = new List<string>();
            public List<string> Parameters { get; set; } = new List<string>();
            public bool RequiresApiKey { get; set; }
        }
    }
}
