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
        #region Discord
        public ulong GuildId { get; set; }
        public ulong ChallengeReleaseChannelId { get; set; }
        public ulong ChallengeSolvesChannelId { get; set; }
        public ulong LeaderboardChannelId { get; set; }
        public ulong TodaysChannelId { get; set; }
        public ulong LogsChannelId { get; set; }
        public ulong BoardChannelId { get; set; }
        public ulong BoardWriteupsChannelId { get; set; }
        public ulong BoardRoleId { get; set; }
        public ulong FirstPlaceRoleId { get; set; }
        public ulong SecondPlaceRoleId { get; set; }
        public ulong ThirdPlaceRoleId { get; set; }
        public ulong TodaysRoleId { get; set; }
        public ulong ChallengePingRoleId { get; set; }
        public int ReleaseTime { get; set; }
        #endregion
        #region Shared
        public bool IsFinished { get; set; }
        #endregion
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
