using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iCTF_Shared_Resources.Models
{
    public class Redirect
    {
        [Key]
        public int Id { get; set; }
        public string RandomId { get; set; }
        public string RedirectUrl { get; set; }
    }
}
