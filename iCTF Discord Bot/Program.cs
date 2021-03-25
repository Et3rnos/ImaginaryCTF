using Discord;
using Discord.Commands;
using Discord.WebSocket;
using iCTF_Shared_Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot
{
    class Program
    {
        private const string _token = "BOT_TOKEN";
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private Program()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _client.Log += Log;
            _commands.Log += Log;
            _services = ConfigureServices();
        }

        private IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddDbContext<DatabaseContext>(options => 
                    options.UseMySql(SharedConfiguration.connectionString,
                    new MySqlServerVersion(new Version(5, 7))));

            return map.BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine($"[{DateTime.UtcNow} - {msg.Severity}] {msg.ToString()}");
            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            await InitCommands();

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            await _client.SetGameAsync("CTF challenges | .help", type: ActivityType.Playing);

            Scheduler.Setup(_services, _client);

            await Task.Delay(Timeout.Infinite);
        }

        private async Task InitCommands()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasStringPrefix(".", ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }
}
