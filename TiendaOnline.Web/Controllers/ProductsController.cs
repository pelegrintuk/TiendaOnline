using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using System.Security.Claims;


namespace TiendaOnline.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public ProductsController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
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
            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : HttpContext.Request.Cookies["TempUserId"];
            if (userId == null)
            {
                userId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append("TempUserId", userId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) });
            }

            var cartItemDto = new CartItemDto
            {
                ProductId = productId,
                Quantity = quantity
            };

            await _cartService.AddToCartAsync(userId, cartItemDto);
            return RedirectToAction("Index", "Cart");
        }

        // Método para crear un nuevo producto
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
                await _productService.CreateProductAsync(productDto);
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);
        }

        // Método para editar un producto existente
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
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(productDto);
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);
        }

        // Método para eliminar un producto existente
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
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
