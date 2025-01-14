using Serilog;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Infrastructure.Logging
{
    public class LogService : ILogService
    {
        public LogService()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void LogInformation(string message)
        {
            Log.Information(message);
        }

        public void LogError(string message, Exception ex)
        {
            Log.Error(ex, message);
        }
    }
}
