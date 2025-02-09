namespace Homework2.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string CatName { get; set; }
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
