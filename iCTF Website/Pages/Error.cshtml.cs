using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iCTF_Website.Pages
{
    public class ErrorModel : PageModel
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public string RooNoBooliUrl { get; set; }
        public string RooYayUrl { get; set; }

        public ErrorModel() { }

        public void OnGet(int code)
        {
            Code = code;
            Title = string.Join(" ", Regex.Split(((HttpStatusCode)code).ToString(), @"(?<!^)(?=[A-Z])"));

            if ((new Random()).Next(10) == 0) {
                RooNoBooliUrl = "https://cdn.discordapp.com/emojis/811894601333342229.png?v=1";
                RooYayUrl = "https://cdn.discordapp.com/emojis/811895872568819743.png?v=1";
            } else {
                RooNoBooliUrl = "https://cdn.discordapp.com/emojis/778310888243200021.png?v=1";
                RooYayUrl = "/img/rooYay.png";
            }
        }
    }
}
