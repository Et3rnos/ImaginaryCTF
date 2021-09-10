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
            var eb = new CustomEmbedBuilder();
            eb.WithThumbnailUrl("https://cdn.discordapp.com/avatars/669798825765896212/572b97a2e8c1dc33265ac51679303c41.png?size=256");
            eb.WithTitle("About");
            eb.AddField("Author", "This bot was created by Et3rnos#6556");
            eb.AddField("Support", "If you want to support me you can visit my Patreon:\n<https://www.patreon.com/et3rnos>");
            await ReplyAsync(embed: eb.Build());
        }

        [Command("help")]
        [Summary("Prints information about available commands")]
        public async Task Help()
        {
            var eb = new CustomEmbedBuilder()
            {
                Title = "Available Commands"
            };

            var mainCommandsNames = new string[] { "flag", "stats" };
            var mainCommands = _commands.Commands.Where(x => mainCommandsNames.Contains(x.Name));

            eb.Description += "\n**Main Commands**";

            foreach (var command in mainCommands)
            {
                eb.Description += $"\n`.{command.Name}";
                foreach (var parameter in command.Parameters)
                {
                    if (parameter.IsOptional)
                        eb.Description += $" [{parameter.Name}]";
                    else
                        eb.Description += $" <{parameter.Name}>";
                }
                eb.Description += "`";
                var requirePermission = command.Preconditions.OfType<RequireUserPermissionAttribute>().FirstOrDefault();
                if (requirePermission != null)
                {
                    eb.Description += " :star:";
                }
                eb.Description += $"\n➜ {command.Summary}";
                var requireContext = command.Preconditions.OfType<RequireContextAttribute>().FirstOrDefault();
                if (requireContext != null)
                {
                    switch (requireContext.Contexts)
                    {
                        case ContextType.DM:
                            eb.Description += " (DMs only)";
                            break;
                        case ContextType.Guild:
                            eb.Description += " (Guild only)";
                            break;
                    }
                }
            }

            eb.Description += "\n\n**Other Commands**";

            foreach (var module in _commands.Modules)
            {
                eb.Description += $"\n`.help {module.Name}`";
            }

            eb.WithFooter("Starred commands require the user to have certain permissions");

            await ReplyAsync(embed: eb.Build());
        }

        [Command("help")]
        [Summary("Prints all available commands in a category")]
        public async Task HelpAdmin([Name("category")] string category)
        {
            var module = _commands.Modules.FirstOrDefault(x => x.Name.ToLower() == category.ToLower());
            if (module == null)
            {
                await ReplyAsync("That's not a valid category name.");
                return;
            }

            var eb = new CustomEmbedBuilder
            {
                Title = "Available Commands",
                Color = Color.Blue
            };

            foreach (var command in module.Commands)
            {
                eb.Description += $"\n`.{command.Name}";
                foreach (var parameter in command.Parameters)
                {
                    if (parameter.IsOptional)
                        eb.Description += $" [{parameter.Name}]";
                    else
                        eb.Description += $" <{parameter.Name}>";
                }
                eb.Description += "`";
                var requirePermission = command.Preconditions.OfType<RequireUserPermissionAttribute>().FirstOrDefault();
                if (requirePermission != null)
                {
                    eb.Description += " :star:";
                }
                eb.Description += $"\n➜ {command.Summary}";
                var requireContext = command.Preconditions.OfType<RequireContextAttribute>().FirstOrDefault();
                if (requireContext != null)
                {
                    switch (requireContext.Contexts)
                    {
                        case ContextType.DM:
                            eb.Description += " (DMs only)";
                            break;
                        case ContextType.Guild:
                            eb.Description += " (Guild only)";
                            break;
                    }
                }
            }

            eb.WithFooter("Starred commands require the user to have certain permissions");

            await ReplyAsync(embed: eb.Build());
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
            await _client.Rest.DeleteAllGlobalCommandsAsync();

            var statsCommand = new SlashCommandBuilder();
            statsCommand.WithName("stats");
            statsCommand.WithDescription("Prints the player statistics");
            statsCommand.AddOption("user", ApplicationCommandOptionType.User, "the user to print the stats", required: false);
            await _client.Rest.CreateGlobalCommand(statsCommand.Build());

            var leaderboardCommand = new SlashCommandBuilder();
            leaderboardCommand.WithName("leaderboard");
            leaderboardCommand.WithDescription("Prints the leaderboard");
            await _client.Rest.CreateGlobalCommand(leaderboardCommand.Build());

            await ReplyAsync("Slash commands were reset. Please give it some time for the changes to take effect.");
        }
    }
}
