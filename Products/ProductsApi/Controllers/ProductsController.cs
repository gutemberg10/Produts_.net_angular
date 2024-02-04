using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Models;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsContext _context;

        public ProductsController(ProductsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/products")]
        [Authorize]
        public async Task<ActionResult> GetProducts()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        [HttpPost]
        [Route("/products")]
        [Authorize]
        public async Task<ActionResult> CreateProduct(Product product)
        {
            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut]
        [Route("/products")]
        [Authorize]
        public async Task<ActionResult> UpdateProduct(Product product)
        {
            var dbProduct = await _context.Products.FindAsync(product.Id);

            if (dbProduct == null)
                return NotFound();

            dbProduct.Name = product.Name;
            dbProduct.Price = product.Price;
            dbProduct.Category = product.Category;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete]
        [Route("/products")]
        [Authorize]
        public async Task<ActionResult> DeletarProduct(Guid id)
        {
            var dbProduct = await _context.Products.FindAsync(id);

            if (dbProduct == null)
                return NotFound();

            _context.Products.Remove(dbProduct);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
