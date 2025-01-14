using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TiendaOnline.Services.Filters
{
    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Ejemplo de validación: verificar un encabezado personalizado
            var authHeader = context.HttpContext.Request.Headers["X-Custom-Auth"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || authHeader != "ExpectedValue")
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}

