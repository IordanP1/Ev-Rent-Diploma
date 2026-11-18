using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace EvRent.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Safe test mode
            if (email.EndsWith("@test.com") || email.Contains("test"))
            {
                Console.WriteLine($"FAKE EMAIL SENT TO: {email}");
                Console.WriteLine($"SUBJECT: {subject}");
                return;
            }

            var settings = _config.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                settings["SenderName"],
                settings["SenderEmail"]
            ));

            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                settings["SmtpServer"],
                int.Parse(settings["Port"]!),
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                settings["Username"],
                settings["Password"]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}