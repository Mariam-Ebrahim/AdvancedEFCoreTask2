
using System.ComponentModel.DataAnnotations;

namespace AdvancedEFCore.Models
{
    public class Product : EntityBase
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public bool IsDeleted { get; set; } = false;

        public int CategoryId { get; set; }

        // Navigation property
        public Category Category { get; set; } = null!;
    }
}
