using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iCTF_Shared_Resources.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Score { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<User> Members { get; set; }
        public List<Challenge> SolvedChallenges { get; set; } = new List<Challenge>();
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
