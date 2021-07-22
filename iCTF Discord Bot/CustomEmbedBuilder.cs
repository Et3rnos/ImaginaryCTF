using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCTF_Discord_Bot
{
    class CustomEmbedBuilder : EmbedBuilder
    {
        public CustomEmbedBuilder()
        {
            WithColor(Discord.Color.Blue);
        }
    }
}
