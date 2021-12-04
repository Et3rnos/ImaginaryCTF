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

        public ErrorModel() { }

        public void OnGet(int code)
        {
            Code = code;
        }

        public void OnPost(int code)
        {
            Code = code;
        }
    }
}
