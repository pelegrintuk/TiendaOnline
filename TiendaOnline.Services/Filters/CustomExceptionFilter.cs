using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TiendaOnline.Services.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            // Crear un resultado uniforme para las excepciones
            var result = new ObjectResult(new
            {
                Message = "Ocurrió un error inesperado.",
                Error = exception.Message,
                StackTrace = exception.StackTrace
            })
            {
                StatusCode = 500
            };

            context.Result = result;
        }
    }
}

