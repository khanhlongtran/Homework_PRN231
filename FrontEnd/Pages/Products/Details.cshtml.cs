using FrontEnd.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEnd.Pages.Products
{
    public class DetailsModel : PageModel
    {

        private readonly HttpClient _httpClient;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public ProductDto Product { get; set; }
        public List<string> ProductImages { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5036/products/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            Product = JsonSerializer.Deserialize<ProductDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (Product != null && !string.IsNullOrEmpty(Product.Images))
            {
                ProductImages = Product.Images.Split(';').ToList();
            }

            return Page();
        }
    }
}
