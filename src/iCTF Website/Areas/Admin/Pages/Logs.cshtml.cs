using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCTF_Shared_Resources.Models;
using iCTF_Shared_Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Logging;

namespace iCTF_Website.Areas.Admin.Pages
{
    [Authorize(Roles = "Administrator")]
    public class LogsModel : PageModel
    {
        public string Logs { get; set; }

        public LogsModel() { }

        public async Task OnGetAsync()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "logs");
            var logs = new DirectoryInfo(path).GetFiles().OrderByDescending(x => x.LastWriteTime).First().FullName;

            using var fstream = new FileStream(logs, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sreader = new StreamReader(fstream);
            var lines = new List<string>();
            string line;
            while ((line = await sreader.ReadLineAsync()) != null)
                lines.Add(line);

            lines = lines.TakeLast(100).Reverse().ToList();

            Logs = string.Join("\n", lines);
        }
    }
}
