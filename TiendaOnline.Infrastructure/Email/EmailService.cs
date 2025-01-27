using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Threading.Tasks;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly bool _isSmtpConfigured;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Verificar si la configuración SMTP es válida
            _isSmtpConfigured = !string.IsNullOrEmpty(_configuration["SmtpSettings:Host"]) &&
                                !string.IsNullOrEmpty(_configuration["SmtpSettings:Port"]);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (_isSmtpConfigured)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["SmtpSettings:Username"] ?? "no-reply@localhost"));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                using var smtp = new SmtpClient();
                try
                {
                    await smtp.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), SecureSocketOptions.None);
                    if (!string.IsNullOrEmpty(_configuration["SmtpSettings:Username"]))
                    {
                        await smtp.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                    }
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending email to {Email}. Simulating email send.", to);
                    // Simular el envío de correo registrándolo en los logs
                    _logger.LogInformation("Simulando el envío de correo a {To} con el asunto {Subject} y el cuerpo {Body}", to, subject, body);
                }
            }
            else
            {
                // Simular el envío de correo registrándolo en los logs
                _logger.LogInformation("Simulando el envío de correo a {To} con el asunto {Subject} y el cuerpo {Body}", to, subject, body);
                await Task.CompletedTask;
            }
        }
    }
}
