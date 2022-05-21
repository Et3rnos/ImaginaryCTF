using Discord;
using Discord.Commands;
using Discord.WebSocket;
using iCTF_Discord_Bot.Logic;
using iCTF_Shared_Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot
{
    class Program
    {
        private IConfigurationRoot configuration;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig {
                GatewayIntents = GatewayIntents.All
            });
            _commands = new CommandService(new CommandServiceConfig {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async
            });
            _services = ConfigureServices();
            _client.Log += LogAsync;
            _commands.Log += LogAsync;
        }

        private IServiceProvider ConfigureServices()
        {
            configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json", false)
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var connectionString = configuration.GetValue<string>("ConnectionString");

            var map = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(configuration)
                .AddDbContext<DatabaseContext>(options =>
                    options.UseMySql(connectionString,
                    ServerVersion.AutoDetect(connectionString))
                );

            return map.BuildServiceProvider();
        }

        #pragma warning disable CS1998
        private async Task LogAsync(LogMessage msg)
        #pragma warning restore CS1998
        {
            var text = msg.ToString();
            switch (msg.Severity)
            {
                case LogSeverity.Debug:
                    Log.Debug(text);
                    break;
                case LogSeverity.Verbose:
                    Log.Debug(text);
                    break;
                case LogSeverity.Info:
                    Log.Information(text);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(text);
                    break;
                case LogSeverity.Error:
                    Log.Error(text);
                    break;
                case LogSeverity.Critical:
                    Log.Fatal(text);
                    break;
            }
        }

        private async Task MainAsync()
        {
            await InitCommands();

            await _client.LoginAsync(TokenType.Bot, configuration.GetValue<string>("Token"));
            await _client.StartAsync();

            await _client.SetGameAsync(".help", type: ActivityType.Playing);

            Scheduler.Setup(_services, _client);

            await Task.Delay(Timeout.Infinite);
        }

        private async Task InitCommands()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += UserJoinedAsync;
            _client.InteractionCreated += InteractionCreatedAsync;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message) return;

            int argPos = 0;

            if (!(message.HasStringPrefix(".", ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            var channel = user.Guild.SystemChannel;
            var eb = new EmbedBuilder
            {
                Color = Color.Blue,
                Title = $"{user.Username} is here!",
                ThumbnailUrl = user.GetAvatarUrl()
            };
            eb.Description = $"Welcome to **ImaginaryCTF**'s discord server!\n\n";
            eb.Description += "**Useful channels**\n";
            eb.Description += ":small_blue_diamond: <#732318420611366933>\n";
            eb.Description += ":small_blue_diamond: <#762690967093379082>\n";
            eb.Description += "\n**Prefer solving challenges through web?**\n";
            eb.Description += "No problem! You can solve them at <https://imaginaryctf.org>\n";
            eb.WithFooter($"You are our #{user.Guild.MemberCount + 1 /*idk if the +1 is necessary, if you know pls message me*/ } member!");
            await channel.SendMessageAsync(embed: eb.Build());
        }

        private async Task InteractionCreatedAsync(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
            {
                await SlashCommands.ExecuteCommandAsync(_services, command);
            }
        }
    }
}
