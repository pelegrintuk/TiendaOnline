using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;


namespace TiendaOnline.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;

        public ProductsController(IProductService productService, ICartService cartService, ILogger<ProductsController> logger, IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory)
        {
            _productService = productService;
            _cartService = cartService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            ViewBag.Category = category;
            return View("Index", products);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search(string query)
        {
            var products = await _productService.SearchProductsAsync(query);
            return View("Index", products);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = GetUserId();

            // Obtener el nombre del producto
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "El producto no existe.";
                return RedirectToAction("Index");
            }

            var cartItemDto = new CartItemDto
            {
                ProductId = productId,
                ProductName = product.Name,
                Quantity = quantity
            };

            _logger.LogInformation("Adding product {ProductId} to cart for user: {UserId}", productId, userId);

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Cart/{userId}", cartItemDto);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Product {ProductId} added to cart for user: {UserId}", productId, userId);
                    return RedirectToAction("Index");
                }

                _logger.LogError("Error adding product {ProductId} to cart for user: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "Error al agregar el producto al carrito.";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for user: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "Error al agregar el producto al carrito.";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                if (productDto.ImageFiles != null && productDto.ImageFiles.Count > 0)
                {
                    foreach (var file in productDto.ImageFiles)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        productDto.Images.Add(new ProductImageDto { ImageUrl = $"/images/{fileName}" });
                    }
                }

                await _productService.CreateProductAsync(productDto);
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDto productDto, int[] ImagesToDelete)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Asignar las imágenes a eliminar al DTO
                productDto.ImagesToDelete = ImagesToDelete;

                // Procesar nuevas imágenes
                if (productDto.ImageFiles != null && productDto.ImageFiles.Count > 0)
                {
                    foreach (var file in productDto.ImageFiles)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        productDto.Images.Add(new ProductImageDto { ImageUrl = $"/images/{fileName}" });
                    }
                }

                await _productService.UpdateProductAsync(productDto);
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar las imágenes existentes
            var existingProduct = await _productService.GetProductByIdAsync(id);
            productDto.Images = existingProduct.Images;

            return View(productDto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                _logger.LogInformation("Product {ProductId} deleted by admin", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                TempData["ErrorMessage"] = "Error al eliminar el producto.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string GetUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else
            {
                var tempUserId = HttpContext.Request.Cookies["TempUserId"];
                if (tempUserId == null)
                {
                    tempUserId = Guid.NewGuid().ToString();
                    HttpContext.Response.Cookies.Append("TempUserId", tempUserId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) });
                }
                return tempUserId;
            }
        }
    }
}
