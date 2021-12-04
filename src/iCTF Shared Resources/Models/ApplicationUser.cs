using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string ApiKey { get; set; }
    }
}
