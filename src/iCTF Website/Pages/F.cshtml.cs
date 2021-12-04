using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using iCTF_Shared_Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace iCTF_Website.Pages
{
    public class FModel : PageModel
    {
        private readonly DatabaseContext _context;

        public FModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string randomId)
        {
            if (!randomId.All(c => char.IsLetterOrDigit(c))) return NotFound();
            string fdownlUrl = $"https://fdow.nl/{randomId}";

            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, fdownlUrl));
            if (response.IsSuccessStatusCode) return Redirect(fdownlUrl);

            var redirect = await _context.Redirects.Where(x => x.RandomId == randomId).FirstOrDefaultAsync();

            if (redirect == null) return NotFound();
            else return Redirect(redirect.RedirectUrl);
        }
    }
}
