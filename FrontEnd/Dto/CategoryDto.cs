using System.Text.Json.Serialization;

namespace FrontEnd.Dto
{
    public class CategoryDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("catName")]
        public string CatName { get; set; }
    }

}
