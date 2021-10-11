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
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace iCTF_Website.Areas.Account.Pages
{
    [Authorize]
    public class DiscordAccountModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public string State { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }
        public ulong DiscordId { get; set; }

        public DiscordAccountModel(DatabaseContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task OnGetAsync(string code, string state)
        {
            var appUser = await _userManager.GetUserAsync(User);
            var wPlayer = await _context.Users.Where(x => x.Id == appUser.UserId).Include(x => x.Solves).ThenInclude(x => x.Challenge).FirstOrDefaultAsync();

            DiscordId = wPlayer.DiscordId;

            var session = Request.Cookies["SESSION"];
            State = Hash(session);

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                return;
            }

            if (state != State)
            {
                Error = "Could not validate your session";
                return;
            }

            string client_id = _configuration.GetValue<string>("DiscordClientId");
            string client_secret = _configuration.GetValue<string>("DiscordClientSecret");
            string redirect_url = _configuration.GetValue<string>("DiscordRedirectUrl");

            var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "client_id", client_id },
                { "client_secret", client_secret },
                { "redirect_uri", redirect_url },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "scope", "identify" }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://discordapp.com/api/oauth2/token", content);
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseString);
            if (string.IsNullOrEmpty((string)data.access_token)) {
                Error = "Invalid code supplied";
                return; 
            }
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (string)data.access_token);
            response = await client.GetAsync("https://discordapp.com/api/users/@me");
            responseString = await response.Content.ReadAsStringAsync();
            data = JsonConvert.DeserializeObject(responseString);
            if (string.IsNullOrEmpty((string)data.id)) {
                Error = "An error occured while fetching your data";
                return;
            }
            ulong discordId = (ulong)data.id;
            string username = (string)data.username;
            string discriminator = (string)data.discriminator;

            var dPlayer = await _context.Users.Where(x => x.DiscordId == discordId).Include(x => x.WebsiteUser).Include(x => x.Solves).ThenInclude(x => x.Challenge).FirstOrDefaultAsync();

            if (dPlayer != null && dPlayer.WebsiteUser != null)
            {
                Error = "There's already a user linked to this discord account";
                return;
            }

            if (dPlayer != null && dPlayer.Id != wPlayer.Id)
            {
                if (dPlayer.Solves.Sum(x => x.Challenge.Points) > wPlayer.Solves.Sum(x => x.Challenge.Points)) {
                    var wSolves = await _context.Solves.Where(x => x.UserId == wPlayer.Id).ToListAsync();
                    _context.Solves.RemoveRange(wSolves);
                    var dSolves = await _context.Solves.Where(x => x.UserId == dPlayer.Id).ToListAsync();
                    foreach (var dSolve in dSolves) {
                        dSolve.UserId = wPlayer.Id;
                    }
                    wPlayer.LastUpdated = dPlayer.LastUpdated;
                }
                _context.Users.Remove(dPlayer);
            }

            wPlayer.DiscordId = discordId;
            wPlayer.DiscordUsername = $"{username}#{discriminator}";
            await _context.SaveChangesAsync();

            DiscordId = wPlayer.DiscordId;
            Success = "Your discord account was successfully linked";
        }

        public string Hash(string plaintext)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
