namespace AdvancedEFCore.DTOs;
using System.ComponentModel.DataAnnotations;

public class UpdateProductDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }
    [Required]
    public byte[]? RowVersion { get; set; }

    public int CategoryId { get; set; }
}