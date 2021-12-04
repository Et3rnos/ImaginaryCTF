using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using iCTF_Discord_Bot.Logic;
using Microsoft.Extensions.Configuration;

namespace iCTF_Discord_Bot.Modules
{
    [Name("user")]
    public class UserModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly DatabaseContext _context;
        private readonly IServiceScope _scope;
        private readonly IConfigurationRoot _configuration;

        private UserModule(DiscordSocketClient client, CommandService commands, IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _commands = commands;
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetService<DatabaseContext>();
            _configuration = _scope.ServiceProvider.GetService<IConfigurationRoot>();
        }

        ~UserModule() { _scope.Dispose(); }

        [Command("stats")]
        [Summary("Prints the player statistics")]
        public async Task Stats([Name("user")] IUser dUser = null)
        {
            await UserLogic.StatsCommandAsync(Context, _client, _context, _configuration, dUser);
        }
    }
}
