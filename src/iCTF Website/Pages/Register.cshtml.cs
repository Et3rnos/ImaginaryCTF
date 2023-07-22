using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using iCTF_Shared_Resources;
using iCTF_Shared_Resources.Models;
using iCTF_Website.Helpers;
using iCTF_Website.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace iCTF_Website.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IRecaptchaService _recaptchaService;

        public RegisterModel(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IConfiguration configuration,
            IEmailService emailService,
            IRecaptchaService recaptchaService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
            _recaptchaService = recaptchaService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }


        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            string reCaptchaResponse = Request.Form["g-recaptcha-response"];
            returnUrl ??= Url.Content("~/");

            if(!await _recaptchaService.VerifyAsync(reCaptchaResponse)) {
                ModelState.AddModelError(string.Empty, "Invalid Recaptcha");
                return Page();
            }

            if (!ModelState.IsValid) return Page();

            var existentUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existentUser != null && !existentUser.EmailConfirmed) await _userManager.DeleteAsync(existentUser);

            var user = new ApplicationUser { UserName = Input.Username, Email = Input.Email, User = new User(), ApiKey = RandomHelper.GenerateRandomString(), VerificationToken = RandomHelper.GenerateRandomString() };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Account", userId = user.Id, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailService.SendAsync(Input.Email, "ImaginaryCTF - Confirm your email",
$@"
Hello {HtmlEncoder.Default.Encode(user.UserName)}!
<br/>
<br/>
Please confirm your account by
<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.
<br/>
<br/>
Best Regards,
<br/>
ImaginaryCTF's Team
");

                return RedirectToPage("/RegisterConfirmation", new { area = "Account" });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
