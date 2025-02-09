using System.Text.Json.Serialization;

namespace FrontEnd.Dto
{
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("pName")]
        public string PName { get; set; }
        [JsonPropertyName("pPrice")]
        public int PPrice { get; set; }
        [JsonPropertyName("pQuantity")]
        public int PQuantity { get; set; }
        [JsonPropertyName("images")]
        public string Images { get; set; }
        public Guid CategoryId { get; set; }
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }
    }

}
