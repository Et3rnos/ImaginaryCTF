using Discord.WebSocket;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using iCTF_Discord_Bot.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot
{
    class Scheduler
    {
        private static IServiceProvider _serviceProvider;
        private static IScheduler _scheduler;
        private static IServiceScopeFactory _scopeFactory;
        private static DiscordSocketClient _client;

        public static void Setup(IServiceProvider serviceProvider, DiscordSocketClient client)
        {
            _serviceProvider = serviceProvider;
            _scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            _client = client;
            UpdateChallengeReleaseJob();
        }

        public static void UpdateChallengeReleaseJob(Config config = null)
        {
            if (config == null) {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<DatabaseContext>();
                config = context.Configuration.FirstOrDefault();
            }
            if (config == null) {
                return;
            }

            var startTime = DateTime.Today.AddMinutes(config.ReleaseTime);
            startTime = startTime > DateTime.UtcNow ? startTime : startTime.AddDays(1);
            startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);

            var warningTime = startTime.Subtract(TimeSpan.FromHours(1));
            warningTime = warningTime > DateTime.UtcNow ? warningTime : warningTime.AddDays(1);
            warningTime = DateTime.SpecifyKind(warningTime, DateTimeKind.Utc);

            if (_scheduler != null) {
                _scheduler.Shutdown();
            }

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            _scheduler.Context.Put("client", _client);
            _scheduler.Context.Put("scopeFactory", _scopeFactory);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("challenge_release_trigger")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24))
                .Build();
            IJobDetail job = JobBuilder.Create<ChallengeReleaseJob>()
                .WithIdentity("challenge_release_job", "main_group")
                .Build();
            _scheduler.ScheduleJob(job, trigger);

            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("announce_website_solves_trigger")
                .WithSimpleSchedule(x => x.RepeatForever().WithIntervalInMinutes(1))
                .Build();
            IJobDetail job2 = JobBuilder.Create<AnnounceWebsiteSolvesJob>()
                .WithIdentity("announce_website_solves_job", "main_group")
                .Build();

            _scheduler.ScheduleJob(job2, trigger2);

            ITrigger trigger3 = TriggerBuilder.Create()
                .WithIdentity("warning_no_approved_challenges")
                .StartAt(warningTime)
                .WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24))
                .Build();
            IJobDetail job3 = JobBuilder.Create<WarnNoApprovedChallsJob>()
                .WithIdentity("warning_no_approved_challenges", "main_group")
                .Build();

            _scheduler.ScheduleJob(job3, trigger3);

            _scheduler.Start();
        }
    }
}
