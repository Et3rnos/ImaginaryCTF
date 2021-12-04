using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using iCTF_Shared_Resources;
using Microsoft.Extensions.DependencyInjection;
using iCTF_Shared_Resources.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace iCTF_Discord_Bot.Modules
{
    [Name("shorten")]
    public class ShortenModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly DatabaseContext _context;
        private readonly IServiceScope _scope;
        private readonly IConfigurationRoot _configuration;

        private ShortenModule(DiscordSocketClient client, CommandService commands, IServiceScopeFactory scopeFactory, IConfigurationRoot configuration)
        {
            _client = client;
            _commands = commands;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
            _configuration = configuration;
        }

        ~ShortenModule() { _scope.Dispose(); }

        [Command("shorten")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [Summary("Shortens an url and make it accessible under /{randomid}-{name}")]
        public async Task Shorten(string url, string name)
        {
            string randomId = GenerateRandomIdentifier() + "-" + name;

            var redirect = new Redirect
            {
                RandomId = randomId,
                RedirectUrl = url
            };
            await _context.Redirects.AddAsync(redirect);
            await _context.SaveChangesAsync();

            await ReplyAsync($"Your file is now accessible at:\n<https://imaginaryctf.org/r/{randomId}>");
        }

        [Command("shorten")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [Summary("Shortens an url and make it accessible under /{randomid}")]
        public async Task Shorten(string url)
        {
            string randomId = GenerateRandomIdentifier();

            var redirect = new Redirect
            {
                RandomId = randomId,
                RedirectUrl = url
            };
            await _context.Redirects.AddAsync(redirect);
            await _context.SaveChangesAsync();

            await ReplyAsync($"Your file is now accessible at:\n<https://imaginaryctf.org/r/{randomId}>");
        }

        [Command("shorten")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [Summary("Presents you with a shortened url to access the file you uploaded")]
        public async Task Shorten()
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();

            if (attachment == null) {
                await ReplyAsync("I could not find an attachment within your message.");
                return;
            }

            string randomId = GenerateRandomIdentifier() + "-" + attachment.Filename;
            string redirectUrl = attachment.Url;

            var redirect = new Redirect {
                RandomId = randomId,
                RedirectUrl = redirectUrl
            };
            await _context.Redirects.AddAsync(redirect);
            await _context.SaveChangesAsync();

            await ReplyAsync($"Your file is now accessible at:\n<https://imaginaryctf.org/r/{randomId}>");
        }

        private string GenerateRandomIdentifier()
        {
            Random random = new Random();
            byte[] randomBytes = new byte[2];
            random.NextBytes(randomBytes);
            string randomHex = BitConverter.ToString(randomBytes).Replace("-", string.Empty);
            return randomHex;
        }

        [Command("fdownl")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        [Summary("Uploads a file to FDownl")]
        public async Task Fdownl()
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment == null) { await ReplyAsync("I could not find an attachment within your message."); return; }

            var httpClient = new HttpClient();
            using var downloadResponse = await httpClient.GetAsync(attachment.Url, HttpCompletionOption.ResponseHeadersRead);
            if (!downloadResponse.IsSuccessStatusCode) { await ReplyAsync("Discord is having problems with their CDN I guess :("); return; }
            using var content = await downloadResponse.Content.ReadAsStreamAsync();

            var form = new MultipartFormDataContent();
            form.Add(new StringContent("604800"), "lifetime");
            var coupon = _configuration.GetValue<string>("FDownlCoupon");
            if (!string.IsNullOrEmpty(coupon)) form.Add(new StringContent(coupon), "code");
            form.Add(new StreamContent(content), "files", attachment.Filename);

            var message = await ReplyAsync("Uploading...");
            var uploadResponse = await httpClient.PostAsync("https://s1.fdow.nl/upload", form);
            await message.DeleteAsync();
            if (!uploadResponse.IsSuccessStatusCode) { await ReplyAsync("Upload failed :("); return; }

            dynamic result = JsonConvert.DeserializeObject(await uploadResponse.Content.ReadAsStringAsync());
            if ((int)result.code != 0) { await ReplyAsync($"The server returned the following error:\n```\n{result.error}\n```"); return; }

            var redirect = new Redirect
            {
                RandomId = (string)result.id,
                RedirectUrl = attachment.Url
            };
            await _context.Redirects.AddAsync(redirect);
            await _context.SaveChangesAsync();

            await ReplyAsync($"Success! Your file is now accessible at:\nhttps://imaginaryctf.org/f/{(string)result.id}");
        }
    }
}
