using AdvancedEFCore.Data;
using AdvancedEFCore.DTOs;
using AdvancedEFCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedEFCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
            int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var query = _context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            var result = await query
                .OrderBy(p => p.Id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    RowVersion = p.RowVersion
                })
                .ToPagedResultAsync(pageNumber, pageSize);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
                return NotFound();

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest($"Category {dto.CategoryId} does not exist.");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;

            _context.Entry(product).Property(p => p.RowVersion).OriginalValue = dto.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("This product was changed by someone else. Reload and try again.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
                return NotFound();

            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
