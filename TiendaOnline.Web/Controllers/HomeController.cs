using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Web.Models;

namespace TiendaOnline.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var featuredProducts = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("api/products/featured");
                var allProducts = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("api/products");
                var categories = allProducts
                    .Select(p => p.Category)
                    .Distinct()
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                ViewBag.Categories = categories;
                return View(featuredProducts);
            }
            catch (HttpRequestException ex)
            {
                // Registrar el error y mostrar un mensaje de error amigable
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<HomeController>>();
                logger.LogError(ex, "Error al realizar la solicitud HTTP a la API");

                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
