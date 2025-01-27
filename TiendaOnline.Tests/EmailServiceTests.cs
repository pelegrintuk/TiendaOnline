using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Infrastructure.Email;
using Xunit;

namespace TiendaOnline.Tests
{
    public class EmailServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<EmailService>> _loggerMock;
        private readonly IEmailService _emailService;

        public EmailServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<EmailService>>();

            // Configurar los valores de configuración simulados
            _configurationMock.SetupGet(c => c["SmtpSettings:Host"]).Returns("smtp.example.com");
            _configurationMock.SetupGet(c => c["SmtpSettings:Port"]).Returns("587");
            _configurationMock.SetupGet(c => c["SmtpSettings:Username"]).Returns("your-email@example.com");
            _configurationMock.SetupGet(c => c["SmtpSettings:Password"]).Returns("your-email-password");

            _emailService = new EmailService(_configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SendEmailAsync_ShouldLogEmailDetails()
        {
            // Arrange
            var to = "test@example.com";
            var subject = "Correo de prueba";
            var body = "Este es un correo de prueba para verificar el servicio de correo.";

            // Act
            await _emailService.SendEmailAsync(to, subject, body);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Simulando el envío de correo")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_ShouldThrowException_WhenEmailIsInvalid()
        {
            // Arrange
            var to = "invalid-email";
            var subject = "Correo de prueba";
            var body = "Este es un correo de prueba para verificar el servicio de correo.";

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _emailService.SendEmailAsync(to, subject, body));
        }
    }
}


