using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using iCTF_Discord_Bot.Managers;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Managers;
using iCTF_Shared_Resources.Models;

namespace iCTF_Discord_Bot
{
    class LeaderboardManager
    {

        public static async Task UpdateLeaderboard(DiscordSocketClient client, DatabaseContext context)
        {
            var config = await context.Configuration.FirstOrDefaultAsync();
            if (config == null || config.GuildId == 0 || config.LeaderboardChannelId == 0) {
                return;
            }
            var channel =  client.GetGuild(config.GuildId).GetTextChannel(config.LeaderboardChannelId);

            var users = await SharedLeaderboardManager.GetTopUsersAndTeams(context, 20);

            var embedBuilder = new CustomEmbedBuilder();
            embedBuilder.WithTitle("Leaderboard");
            embedBuilder.WithDescription("For the full leaderboard please visit <https://imaginaryctf.org/leaderboard>");

            for (int i = 0; i < users.Count; i++) {
                if (users[i].IsTeam)
                {
                    embedBuilder.Description += $"\n**{i + 1}. {users[i].TeamName}** (team) - {users[i].Score} points";
                }
                else
                {
                    embedBuilder.Description += $"\n**{i + 1}. {users[i].WebsiteUser?.UserName ?? users[i].DiscordUsername}** - {users[i].Score} points";
                }
            }

            var message = (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();

            if (message != null && message.Author.Id == client.CurrentUser.Id) {
                await ((IUserMessage)message).ModifyAsync(x => x.Embed = embedBuilder.Build());
            } else {
                await channel.SendMessageAsync(embed: embedBuilder.Build());
            }
        }
    }
}
