using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TiendaOnline.Services.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(m => m.Value.Errors.Any())
                    .Select(m => new
                    {
                        Field = m.Key,
                        Errors = m.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    });

                context.Result = new BadRequestObjectResult(new
                {
                    Message = "La solicitud contiene errores de validación.",
                    Errors = errors
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No se necesita implementar nada aquí para este filtro
        }
    }
}

