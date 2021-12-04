using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCTF_Shared_Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace iCTF_Website.Pages
{
    public class RModel : PageModel
    {
        private readonly DatabaseContext _context;

        public RModel(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string randomId)
        {
            var redirect = await _context.Redirects.Where(x => x.RandomId == randomId).FirstOrDefaultAsync();

            if (redirect == null) { return NotFound(); }
            else { return Redirect(redirect.RedirectUrl); }
        }
    }
}
