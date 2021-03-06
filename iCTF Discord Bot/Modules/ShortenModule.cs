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

namespace iCTF_Discord_Bot.Modules
{
    [Name("shorten")]
    public class ShortenModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly DatabaseContext _context;
        private readonly IServiceScope _scope;

        private ShortenModule(DiscordSocketClient client, CommandService commands, IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _commands = commands;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
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
    }
}
