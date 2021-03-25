using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iCTF_Shared_Resources.Models
{
    public class Solve
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int ChallengeId { get; set; }
        public string ChallengeTitle { get; set; }
        public DateTime SolvedAt { get; set; }
        public bool Announced { get; set; }
    }
}
