using Discord;
using Discord.WebSocket;
using iCTF_Discord_Bot.Managers;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot.Jobs
{
    class AnnounceWebsiteSolvesJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var schedulerContext = context.Scheduler.Context;
            var client = (DiscordSocketClient)schedulerContext.Get("client");
            var scopeFactory = (IServiceScopeFactory)schedulerContext.Get("scopeFactory");

            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DatabaseContext>();

            await SolvesManager.AnnounceWebsiteSolves(client, dbContext);
        }
    }
}
