namespace Homework2.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string PName { get; set; }
        public int PPrice { get; set; }
        public int PQuantity { get; set; }
        public string? Images { get; set; }  // Lưu đường dẫn ảnh
        public Guid CategoryId { get; set; }  // Lưu ID thay vì Object
        public Category Category { get; set; }  // Navigation Property
    }
}
