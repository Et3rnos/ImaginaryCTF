using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iCTF_Shared_Resources.Models
{
    public class Config
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChallengeReleaseChannelId { get; set; }
        public ulong ChallengeSolvesChannelId { get; set; }
        public ulong LeaderboardChannelId { get; set; }
        public ulong TodaysChannelId { get; set; }
        public ulong LogsChannelId { get; set; }
        public ulong FirstPlaceRoleId { get; set; }
        public ulong SecondPlaceRoleId { get; set; }
        public ulong ThirdPlaceRoleId { get; set; }
        public ulong TodaysRoleId { get; set; }
        public uint ReleaseTime { get; set; }
    }
}
