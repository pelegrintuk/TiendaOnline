using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace TiendaOnline.Infrastructure.Email
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.tuservidor.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "usuario@tudominio.com";
        private readonly string _smtpPass = "tucontraseña";

        public void SendEmail(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Tienda Online", "no-reply@tudominio.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(_smtpUser, _smtpPass);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}

