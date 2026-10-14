using AdvancedEFCore.Controllers;
using AdvancedEFCore.DTOs;
using AdvancedEFCore.Helpers;
using AdvancedEFCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AdvancedEFCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController( IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
            int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var result = await _productService.GetPagedAsync(pageNumber, pageSize, search);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(id, dto);

            return result switch
            {
                OperationResult.Success => NoContent(),
                OperationResult.NotFound => NotFound(),
                OperationResult.InvalidCategory => BadRequest($"Category {dto.CategoryId} does not exist."),
                OperationResult.Conflict => Conflict("This product was changed by someone else. Reload and try again."),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);

            if (result == OperationResult.NotFound)
                return NotFound();

            return NoContent();
        }
    }
}
