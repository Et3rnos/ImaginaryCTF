using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;
using Discord;
using iCTF_Discord_Bot.Logic;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace iCTF_Discord_Bot
{
    public static class SlashCommands
    {
        public static async Task ExecuteCommandAsync(IServiceProvider serviceProvider, SocketSlashCommand command)
        {
            var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var services = scope.ServiceProvider;
            var client = services.GetService<DiscordSocketClient>();
            var commandService = services.GetService<CommandService>();
            var dbContext = scope.ServiceProvider.GetService<DatabaseContext>();
            var configuration = scope.ServiceProvider.GetService<IConfigurationRoot>();

            switch (command.Data.Name)
            {
                case "stats":
                    await UserLogic.StatsSlashAsync(command, client, dbContext, configuration);
                    break;
                case "leaderboard":
                    await LeaderboardLogic.LeaderboardSlashAsync(command, dbContext, configuration);
                    break;
                case "about":
                    await UtilLogic.AboutSlashAsync(command);
                    break;
                case "help":
                    await UtilLogic.HelpSlashAsync(command, commandService);
                    break;
            }
        }

        public static async Task ResetCommandsAsync(DiscordSocketClient client)
        {
            await client.Rest.DeleteAllGlobalCommandsAsync();

            var statsCommand = new SlashCommandBuilder();
            statsCommand.WithName("stats");
            statsCommand.WithDescription("Prints the player statistics");
            statsCommand.AddOption("user", ApplicationCommandOptionType.User, "the user to print the stats", required: false);
            await client.Rest.CreateGlobalCommand(statsCommand.Build());

            var leaderboardCommand = new SlashCommandBuilder();
            leaderboardCommand.WithName("leaderboard");
            leaderboardCommand.WithDescription("Prints the leaderboard");
            await client.Rest.CreateGlobalCommand(leaderboardCommand.Build());

            var aboutCommand = new SlashCommandBuilder();
            aboutCommand.WithName("about");
            aboutCommand.WithDescription("Prints information about the bot and its author");
            await client.Rest.CreateGlobalCommand(aboutCommand.Build());

            var helpCommand = new SlashCommandBuilder();
            helpCommand.WithName("help");
            helpCommand.WithDescription("Prints all available commands in a category");
            helpCommand.AddOption("category", ApplicationCommandOptionType.String, "the category to view its commands", required: false);
            await client.Rest.CreateGlobalCommand(helpCommand.Build());
        }
    }
}
