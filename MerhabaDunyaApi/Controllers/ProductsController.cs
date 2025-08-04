using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerhabaDunyaApi.Data;
using MerhabaDunyaApi.Dtos;
using MerhabaDunyaApi.Models;

namespace MerhabaDunyaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        public ProductsController(AppDbContext ctx) => _ctx = ctx;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _ctx.Products.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prod = await _ctx.Products.FindAsync(id);
            if (prod == null) return NotFound();
            return Ok(prod);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
            using var stream = System.IO.File.Create(path);
            await dto.Image.CopyToAsync(stream);

            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                SellerId = dto.SellerId,
                SellerName = dto.SellerName,
                Price = dto.Price,
                Weight = dto.Weight,
                ImageUrl = $"/images/{fileName}",
                Carbon = dto.Weight * 0.5
            };

            _ctx.Products.Add(product);
            await _ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
    }
}
