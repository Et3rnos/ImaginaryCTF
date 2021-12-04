using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using iCTF_Shared_Resources.Models;
using iCTF_Website.Services;

namespace iCTF_Website.Pages
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IRecaptchaService _recaptchaService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailService emailService, IRecaptchaService recaptchaService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _recaptchaService = recaptchaService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Enter your email")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (!await _recaptchaService.VerifyAsync(Request.Form["g-recaptcha-response"])) { ModelState.AddModelError(string.Empty, "Invalid Recaptcha"); return Page(); }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { code },
                protocol: Request.Scheme);

            await _emailService.SendAsync(Input.Email, "Reset Password",
$@"
Hello {HtmlEncoder.Default.Encode(user.UserName)}!
<br/>
<br/>
Please reset your password by
<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.
<br/>
<br/>
Best Regards,
<br/>
ImaginaryCTF's Team
");
            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
