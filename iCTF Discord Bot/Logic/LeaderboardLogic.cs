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

namespace iCTF_Discord_Bot.Logic
{
    public static class LeaderboardLogic
    {
        #region Leaderboard
        public static async Task LeaderboardSlashAsync(SocketSlashCommand command, DatabaseContext dbContext)
        {
            var message = await GetLeaderboardMessageAsync(dbContext);
            await command.RespondAsync(embed: message.Embed, component: message.MessageComponent, ephemeral: message.Ephemeral);
        }

        public static async Task LeaderboardCommandAsync(SocketCommandContext context, DatabaseContext dbContext)
        {
            var message = await GetLeaderboardMessageAsync(dbContext);
            await context.Channel.SendMessageAsync(embed: message.Embed, component: message.MessageComponent);
        }

        private class Message
        {
            public Embed Embed { get; set; }
            public MessageComponent MessageComponent { get; set; }
            public bool Ephemeral { get; set; }
        }

        private static async Task<Message> GetLeaderboardMessageAsync(DatabaseContext dbContext)
        {
            var users = await SharedLeaderboardManager.GetTopUsersAndTeams(dbContext, 20);

            var eb = new CustomEmbedBuilder();
            eb.WithTitle("Leaderboard");

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].IsTeam) eb.Description += $"\n**{i + 1}. {users[i].TeamName}** (team) - {users[i].Score} points";
                else eb.Description += $"\n**{i + 1}. {users[i].WebsiteUser?.UserName ?? users[i].DiscordUsername}** - {users[i].Score} points";
            }

            var builder = new ComponentBuilder().WithButton("Full Leaderboard", style: ButtonStyle.Link, url: "https://imaginaryctf.org/leaderboard");
            return new Message { Embed = eb.Build(), MessageComponent = builder.Build(), Ephemeral = true };
        }
        #endregion
    }
}
