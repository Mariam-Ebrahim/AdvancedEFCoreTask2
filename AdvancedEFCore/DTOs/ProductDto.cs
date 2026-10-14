namespace AdvancedEFCore.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public string CategoryName { get; set; } = string.Empty;
    }
}
