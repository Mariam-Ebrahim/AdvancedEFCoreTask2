namespace AdvancedEFCore.Models
{
    public class Category : EntityBase
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // One category can have many products
        public ICollection<Product> Products { get; set; }
            = new List<Product>();

    }
}
