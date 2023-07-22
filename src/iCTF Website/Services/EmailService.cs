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
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
        Task SendAsync(string to, string subject, string html);
    }

    public class EmailService : IEmailService
    {
        private readonly record struct EmailSettings(string SmtpDomain, int SmtpPort, string Email, string Username, string Password);

        private readonly IConfiguration _configuration;
        private readonly EmailSettings _settings;

        public EmailService(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetService<IConfiguration>();
            _settings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
        }

        public void Send(string to, string subject, string html)
        {
            var email = getMessage(to, _settings.Email, subject, html);

            using var smtp = new SmtpClient();
            smtp.Connect(_settings.SmtpDomain, _settings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(userName: _settings.Username, password: _settings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var email = getMessage(to, _settings.Email, subject, html);

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.SmtpDomain, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(userName: _settings.Username, password: _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private MimeMessage getMessage(string to, string from, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            return email;
        }
    }
}