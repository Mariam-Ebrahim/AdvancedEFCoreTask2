using AdvancedEFCore.DTOs;

namespace AdvancedEFCore.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber, int pageSize, string? search);
    Task<OperationResult> UpdateAsync(int id, UpdateProductDto dto);
    Task<OperationResult> DeleteAsync(int id);
}