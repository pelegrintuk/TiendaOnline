using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaOnline.Application.Interfaces
{
    public interface ILogService
    {
        void LogInformation(string message);
        void LogError(string message, Exception exception);
    }
}
