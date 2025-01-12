using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener productos destacados
            var featuredProducts = await _productService.GetFeaturedProductsAsync();

            // Obtener categorías únicas desde los productos
            var allProducts = await _productService.GetAllProductsAsync();
            var categories = allProducts
                .Select(p => p.Category)
                .Distinct()
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList();

            ViewBag.Categories = categories; // Pasar categorías a la vista
            return View(featuredProducts);
        }
    }
}
