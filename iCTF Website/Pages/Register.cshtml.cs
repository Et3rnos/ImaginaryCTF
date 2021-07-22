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

        public RegisterModel(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
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

            if(!await IsRecaptchaValid(reCaptchaResponse)) {
                ModelState.AddModelError(string.Empty, "Invalid Recaptcha");
                return Page();
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Username, Email = Input.Email, User = new User(), ApiKey = RandomHelper.GenerateRandomString() };
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
            }

            return Page();
        }

        private async Task<bool> IsRecaptchaValid(string reCaptchaResponse)
        {
            if (string.IsNullOrEmpty(reCaptchaResponse)) { return false; }

            var client = new HttpClient();
            var secretKey = _configuration.GetValue<string>("RecaptchaV3SecretKey");
            var values = new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", reCaptchaResponse }
            };
            var data = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", data);
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseString);
            return (bool)json.success;
        }

        private static string GenerateApiKey()
        {
            using var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            var base64 = Convert.ToBase64String(bytes);
            base64 = base64.Replace("=", string.Empty);
            base64 = base64.Replace("/", "-");
            base64 = base64.Replace("+", "_");
            return base64;
        }
    }
}
