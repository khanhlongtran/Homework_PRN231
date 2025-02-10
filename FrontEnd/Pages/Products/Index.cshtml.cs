using FrontEnd.Dto;
using Homework2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEnd.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public Dictionary<Guid, List<string>> ProductImages { get; set; } = new();  // Lưu danh sách ảnh theo ProductId

        public List<ProductDto> Products { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        /// <summary>
        /// GET list products
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("http://localhost:5036/products");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Products = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ProductDto>();

                // Xử lý tách images thành List<string>
                ProductImages = Products.ToDictionary(
                    p => p.Id,
                    p => string.IsNullOrEmpty(p.Images) ? new List<string>() : p.Images.Split(";").ToList()
                );
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:5036/products/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error deleting product.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }


}
