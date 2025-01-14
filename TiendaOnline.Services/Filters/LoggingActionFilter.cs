using Microsoft.AspNetCore.Mvc.Filters;

namespace TiendaOnline.Services.Filters
{
    public class LoggingActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Registrar parámetros de entrada
            var parameters = context.ActionArguments;
            Console.WriteLine($"Parámetros de entrada: {string.Join(", ", parameters.Select(p => $"{p.Key}: {p.Value}"))}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Registrar resultado de la acción
            var result = context.Result;
            Console.WriteLine($"Resultado de la acción: {result}");
        }
    }
}

