namespace TiendaOnline.Application.Interfaces
{
    public interface ILogService
    {
        void LogInformation(string message);
        void LogError(string message, Exception exception);
    }
}
