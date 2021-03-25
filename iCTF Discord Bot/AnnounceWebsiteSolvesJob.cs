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

namespace iCTF_Discord_Bot
{
    class AnnounceWebsiteSolvesJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            SchedulerContext schedulerContext = context.Scheduler.Context;
            DiscordSocketClient client = (DiscordSocketClient)schedulerContext.Get("client");
            IServiceScopeFactory scopeFactory = (IServiceScopeFactory)schedulerContext.Get("scopeFactory");

            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DatabaseContext>();

            await SolvesManager.AnnounceWebsiteSolves(client, dbContext);
        }
    }
}
