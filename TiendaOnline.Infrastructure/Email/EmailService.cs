using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TiendaOnline", "noreply@tiendaonline.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
            await smtpClient.ConnectAsync("smtp.tiendaonline.com", 587, false);
            await smtpClient.AuthenticateAsync("noreply@tiendaonline.com", "yourpassword");
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
    }
}

