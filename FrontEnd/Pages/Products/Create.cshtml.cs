using FrontEnd.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FrontEnd.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [BindProperty] public string PName { get; set; }
        [BindProperty] public int PPrice { get; set; }
        [BindProperty] public int PQuantity { get; set; }
        [BindProperty] public Guid CategoryId { get; set; }
        [BindProperty] public List<IFormFile> ImageFiles { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        /// <summary>
        /// GET Categories
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGet()
        {
            var response = await _httpClient.GetAsync("http://localhost:5036/categories");
            if (!response.IsSuccessStatusCode)
                return Page();

            var json = await response.Content.ReadAsStringAsync();
            var categoryList = JsonSerializer.Deserialize<List<CategoryDto>>(json);

            Categories = categoryList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CatName
            });

            return Page();
        }

        /// <summary>
        /// POST product
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPost()
        {

            using var content = new MultipartFormDataContent();
            // mapping data để gửi lên 
            content.Add(new StringContent(PName), "pName");
            content.Add(new StringContent(PPrice.ToString()), "pPrice");
            content.Add(new StringContent(PQuantity.ToString()), "pQuantity");
            content.Add(new StringContent(CategoryId.ToString()), "categoryId");


            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                foreach (var image in ImageFiles)
                {
                    var stream = image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                    content.Add(fileContent, "imageFiles", image.FileName);
                }
            }
            var response = await _httpClient.PostAsync("http://localhost:5036/products", content);

            return RedirectToPage("Index");
        }
    }
}
