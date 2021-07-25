using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iCTF_Shared_Resources.Models
{
    public class UserTeamUnion
    {
        [Key]
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public string DiscordUsername { get; set; }
        public string TeamName { get; set; }
        public string TeamCode { get; set; }
        public int Score { get; set; }
        public DateTime LastUpdated { get; set; }
        public ApplicationUser WebsiteUser { get; set; }
        public Team Team { get; set; }
        public List<User> Members { get; set; }
        public int MembersCount { get; set; }
        public List<Solve> Solves { get; set; } = new List<Solve>();
        public int SolvesCount { get; set; }
        public bool IsTeam { get; set; }
    }
}
