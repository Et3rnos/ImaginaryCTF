﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCTF_Shared_Resources.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
    }
}
