using Microsoft.AspNetCore.Mvc;

namespace TiendaOnline.Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Renderiza Views/Cart/Index.cshtml
        }
    }
}
