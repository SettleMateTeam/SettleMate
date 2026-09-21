using Microsoft.Extensions.Options;
using SettleMate.Models.Email;
using System.Net;
using System.Net.Mail;

namespace SettleMate.S_services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(
            IOptions<EmailSettings> emailSettings)
        {
            this.emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            using var message = new MailMessage();

            message.From = new MailAddress(
                emailSettings.FromEmail,
                emailSettings.FromName);

            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(
                emailSettings.Host,
                emailSettings.Port);

            smtpClient.EnableSsl =
                emailSettings.EnableSsl;

            smtpClient.Credentials =
                new NetworkCredential(
                    emailSettings.Username,
                    emailSettings.Password);

            await smtpClient.SendMailAsync(message);
        }
    }
}