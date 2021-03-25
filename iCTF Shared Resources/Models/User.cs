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
        public string WebsiteUsername { get; set; }
        public string DiscordUsername { get; set; }
        public uint Score { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
