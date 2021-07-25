using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iCTF_Shared_Resources.Models
{
    public class ChallengeInfo
    {
        public Challenge Challenge { get; set; }
        public int SolvesCount { get; set; }
    }
}
