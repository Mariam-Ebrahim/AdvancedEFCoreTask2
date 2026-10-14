using AdvancedEFCore.Data;
using AdvancedEFCore.DTOs;
using AdvancedEFCore.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AdvancedEFCore.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(
        int pageNumber, int pageSize, string? search)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        return await query
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
    }

    public async Task<OperationResult> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
            return OperationResult.NotFound;

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            return OperationResult.InvalidCategory;

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
            return OperationResult.Conflict;
        }

        return OperationResult.Success;
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
            return OperationResult.NotFound;

        product.IsDeleted = true;
        await _context.SaveChangesAsync();

        return OperationResult.Success;
    }
}