using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var isAdmin = User.IsInRole("Admin");
            var products = await _productService.GetAllProductsAsync(isAdmin);
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { productId = productDto.Id }, productDto);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductDto productDto)
        {
            if (productId != productDto.Id) return BadRequest();

            await _productService.UpdateProductAsync(productDto);
            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }
    }
}
