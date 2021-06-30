using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace iCTF_Website.Services
{
    public interface IEmailService {
        void Send(string to, string subject, string html);
        Task SendAsync(string to, string subject, string html);
    }

    public class EmailService : IEmailService {

        private readonly IConfiguration _configuration;

        public EmailService(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetService<IConfiguration>();
        }

        public void Send(string to, string subject, string html)
        {
            var settings = _configuration.GetSection("EmailSettings");
            string emailAddress = settings.GetValue<string>("Email");
            string password = settings.GetValue<string>("Password");
            string smtpDomain = settings.GetValue<string>("SmtpDomain");
            int smtpPort = settings.GetValue<int>("SmtpPort");

            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailAddress));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(smtpDomain, smtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailAddress, password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var settings = _configuration.GetSection("EmailSettings");
            string emailAddress = settings.GetValue<string>("Email");
            string password = settings.GetValue<string>("Password");
            string smtpDomain = settings.GetValue<string>("SmtpDomain");
            int smtpPort = settings.GetValue<int>("SmtpPort");

            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailAddress));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpDomain, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailAddress, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}