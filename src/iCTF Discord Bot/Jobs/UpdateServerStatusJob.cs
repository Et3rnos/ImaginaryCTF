using Discord;
using Discord.WebSocket;
using iCTF_Discord_Bot.Managers;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot.Jobs
{
    class UpdateServerStatusJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;

            var schedulerContext = context.Scheduler.Context;
            var client = (DiscordSocketClient)schedulerContext.Get("client");
            var scopeFactory = (IServiceScopeFactory)schedulerContext.Get("scopeFactory");

            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DatabaseContext>();

            var config = await dbContext.Configuration.FirstOrDefaultAsync();
            if (config == null || config.GuildId == 0 || config.ServerStatusChannelId == 0)
                return;

            var channel = client.GetGuild(config.GuildId).GetTextChannel(config.ServerStatusChannelId);
            var message = (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();

            var embedBuilder = new CustomEmbedBuilder();

            var metrics = GetMemoryMetrics();

            embedBuilder.WithTitle("Server Status");

            embedBuilder.WithDescription($"**RAM Usage**\n {metrics.Used}MB/{metrics.Total}MB\n\n**CPU Usage**\n {GetCpuUsage()}%");

            embedBuilder.WithFooter($"Last updated at {DateTime.Now}");

            if (message != null && message.Author.Id == client.CurrentUser.Id)
            {
                await ((IUserMessage)message).ModifyAsync(x => x.Embed = embedBuilder.Build());
            }
            else
            {
                await channel.SendMessageAsync(embed: embedBuilder.Build());
            }
        }

        //Credits to https://gunnarpeipman.com/dotnet-core-system-memory/

        public class MemoryMetrics
        {
            public double Total;
            public double Used;
            public double Free;
        }

        private static MemoryMetrics GetMemoryMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m")
            {
                FileName = "/bin/bash",
                Arguments = "-c \"free -m\"",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics
            {
                Total = double.Parse(memory[1]),
                Used = double.Parse(memory[2]),
                Free = double.Parse(memory[3])
            };

            return metrics;
        }

        private static string GetCpuUsage()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m")
            {
                FileName = "/bin/bash",
                Arguments = "-c \"awk '{u=$2+$4; t=$2+$4+$5; if (NR==1){u1=u; t1=t;} else print ($2+$4-u1) * 100 / (t-t1); }'  <(grep 'cpu ' /proc/stat) <(sleep 1;grep 'cpu ' /proc/stat)\"",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            return output.Trim();
        }
    }
}
