using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
using iCTF_Shared_Resources.Models;
using iCTF_Website.Attributes;
using iCTF_Website.Helpers;
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
    public class UserController : Controller {

        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(DatabaseContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> SubmitAsync([FromBody] VerifyModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.VerificationToken == model.Token);
            if (user == null)
                return BadRequest(new { error = "Invalid Verification Token." });

            user.VerificationToken = RandomHelper.GenerateRandomString();
            await _context.SaveChangesAsync();
            return Ok(new
            {
                user_id = user.UserId,
                user_name = user.UserName
            });
        }

        public class VerifyModel
        {
            [Required]
            public string Token { get; set; }
        }
    }
}
