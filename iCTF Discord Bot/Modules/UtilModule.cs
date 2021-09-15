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
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Security.Cryptography;
using iCTF_Discord_Bot.Logic;

namespace iCTF_Discord_Bot.Modules
{
    [Name("util")]
    public class UtilModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly DatabaseContext _context;
        private readonly IServiceScope _scope;

        private UtilModule(DiscordSocketClient client, CommandService commands, IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _commands = commands;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
        }

        ~UtilModule() { _scope.Dispose(); }

        [Command("about")]
        [Summary("Prints information about the bot and its author")]
        public async Task About()
        {
            await UtilLogic.AboutCommandAsync(Context);
        }

        [Command("help")]
        [Summary("Prints information about available commands")]
        public async Task Help()
        {
            await UtilLogic.HelpCommandAsync(Context, _commands);
        }

        [Command("help")]
        [Summary("Prints all available commands in a category")]
        public async Task HelpCategory([Name("category")] string category)
        {
            await UtilLogic.HelpCommandAsync(Context, _commands, category);
        }

        [Command("logs")]
        [Summary("Prints bot logs")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Logs([Name("lines_count")] int linesCount = 10)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "logs");
            var logs = new DirectoryInfo(path).GetFiles().OrderByDescending(x => x.LastWriteTime).First().FullName;

            using var fstream = new FileStream(logs, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sreader = new StreamReader(fstream);
            var lines = new List<string>();
            string line;
            while ((line = await sreader.ReadLineAsync()) != null)
                lines.Add(line);

            lines = lines.TakeLast(linesCount).ToList();
            var msg = $"```\n{string.Join("\n", lines)}\n```";
            if (msg.Length < 2000)
                await ReplyAsync(msg);
            else
                await ReplyAsync("The message is too big to be sent here.");
        }

        [Command("iq")]
        [Summary("Calculates a user's IQ")]
        public async Task Iq(IUser user = null)
        {
            if (user == null)
                user = Context.User;

            ulong id = user.Id;
            int month = DateTime.UtcNow.Month;
            string plaintext = "Who?" + id.ToString() + month.ToString() + "Tao";
            byte[] bytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            uint random = BitConverter.ToUInt32(hash.Take(4).ToArray());
            float x = random / (uint.MaxValue / 10f);
            float y = 1 + MathF.Tan((x * MathF.PI) / 20);
            int iq = (int)(y * 75);
            await ReplyAsync($"{user.Username}'s IQ is {iq}");
        }

        [Command("ping")]
        [Summary("Prints the latency of the bot")]
        public async Task Ping()
        {
            await ReplyAsync($"The latency is currently {_client.Latency}ms.");
        }

        [Command("resetslashcommands")]
        [Summary("Resets slash commands.")]
        [RequireOwner]
        public async Task ResetSlashCommands()
        {
            await SlashCommands.ResetCommandsAsync(_client);
            await ReplyAsync("Slash commands were reset. Please give it some time for the changes to take effect.");
        }
    }
}
