using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot.Managers
{
    public static class LinkedGuildManager
    {
        public enum LinkState
        {
            NotLinked,
            LinkedToOtherGuild,
            LinkedToThisGuild 
        }

        public static async Task<LinkState> GetLinkState(DatabaseContext context, ulong guildId)
        {
            Config config = await context.Configuration.FirstOrDefaultAsync();
            
            if (config == null)
            {
                return LinkState.NotLinked;
            }

            if (config.GuildId == guildId)
            {
                return LinkState.LinkedToThisGuild;
            }

            return LinkState.LinkedToOtherGuild;
        }
    }
}
