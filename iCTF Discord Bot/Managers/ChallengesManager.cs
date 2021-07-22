using Discord;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot.Managers
{
    class ChallengesManager
    {

        public static async Task<Challenge> GetChallengeToBeReleased(DatabaseContext context, bool release = false)
        {
            Challenge chall = await context.Challenges.ToAsyncEnumerable().Where(x => x.State == 1).OrderBy(x => x.Priority).FirstOrDefaultAsync();
            
            if (release && chall != null)
            {
                chall.State = 2;
                chall.ReleaseDate = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }

            return chall;
        }

        public static Embed GetChallengeEmbed(Challenge chall)
        {
            EmbedBuilder builder = new EmbedBuilder();
            Color color;
            if (chall.Points <= 50)
            {
                color = Color.Green;
            }
            else if (chall.Points <= 100)
            {
                color = Color.Gold;
            }
            else if (chall.Points <= 150)
            {
                color = Color.Orange;
            }
            else
            {
                color = Color.Red;
            }
            builder.WithColor(color);
            builder.WithTitle(chall.Title);
            builder.WithDescription(chall.Description);
            builder.AddField("Attachments", chall.Attachments);
            builder.AddField("Category", chall.Category);
            builder.AddField("Author", chall.Author, true);
            builder.AddField("Points", chall.Points, true);
            return builder.Build();
        }
    }
}
