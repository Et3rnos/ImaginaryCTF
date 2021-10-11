using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iCTF_Shared_Resources.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public string DiscordUsername { get; set; }
        public DateTime LastUpdated { get; set; }
        public ApplicationUser WebsiteUser { get; set; }
        public List<Solve> Solves { get; set; } = new List<Solve>();
        public Team Team { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
