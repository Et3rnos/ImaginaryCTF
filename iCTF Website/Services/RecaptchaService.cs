using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iCTF_Website.Services
{
    public interface IRecaptchaService {
        Task<bool> VerifyAsync(string response);
    }

    public class RecaptchaService : IRecaptchaService {

        private readonly IConfiguration _configuration;

        public RecaptchaService(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetService<IConfiguration>();
        }

        public async Task<bool> VerifyAsync(string reCaptchaResponse)
        {
            if (string.IsNullOrEmpty(reCaptchaResponse)) { return false; }

            var client = new HttpClient();
            var secretKey = _configuration.GetValue<string>("RecaptchaV2SecretKey");
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
    }
}