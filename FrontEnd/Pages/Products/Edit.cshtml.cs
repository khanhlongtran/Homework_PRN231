using FrontEnd.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FrontEnd.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [BindProperty] public Guid Id { get; set; }
        [BindProperty] public string PName { get; set; }
        [BindProperty] public int PPrice { get; set; }
        [BindProperty] public int PQuantity { get; set; }
        [BindProperty] public Guid CategoryId { get; set; }
        [BindProperty] public string Images { get; set; }
        [BindProperty] public List<IFormFile> ImageFiles { get; set; }
        public List<SelectListItem> Categories { get; set; } = new();
        [BindProperty] public List<string> ImageList { get; set; } = new();
        [BindProperty] public List<string> DeletedImages { get; set; } = new();

        public async Task<IActionResult> OnGet(Guid id)
        {
            // Lấy dữ liệu sản phẩm từ API
            var response = await _httpClient.GetAsync($"http://localhost:5036/products/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var product = JsonSerializer.Deserialize<ProductDto>(await response.Content.ReadAsStringAsync());

            Id = product.Id;
            PName = product.PName;
            PPrice = product.PPrice;
            PQuantity = product.PQuantity;
            CategoryId = product.CategoryId;
            Images = product.Images;

            // Split chuỗi images thành danh sách
            ImageList = string.IsNullOrEmpty(product.Images)
               ? new List<string>()
               : product.Images.Split(";").ToList();

            // Lấy danh sách cate
            var categoryResponse = await _httpClient.GetAsync("http://localhost:5036/categories");
            if (!categoryResponse.IsSuccessStatusCode)
                return Page();

            var categoryList = JsonSerializer.Deserialize<List<CategoryDto>>(await categoryResponse.Content.ReadAsStringAsync());

            Categories = categoryList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CatName
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {

            // Lấy danh sách ảnh bị xóa từ form vì mình không tạo prop trong model nên làm thủ công tự lấy từ form.
            // Nếu không cần thì tạo prop trong model rồi binding là được
            //var deletedImages = Request.Form["DeletedImages"].ToString()?.Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList();
            var deletedImages = DeletedImages[0].Split(",");
            var x = ImageList.Count();
            ImageList = string.IsNullOrEmpty(ImageList[0])
            ? new List<string>()
            : ImageList[0].Split(';').ToList();
            // Lấy danh sách ảnh còn lại
            var remainingImages = ImageList.Where(img => deletedImages == null || !deletedImages.Contains(img)).ToList();


            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(Id.ToString()), "id");
            content.Add(new StringContent(PName), "pName");
            content.Add(new StringContent(PPrice.ToString()), "pPrice");
            content.Add(new StringContent(PQuantity.ToString()), "pQuantity");
            content.Add(new StringContent(CategoryId.ToString()), "categoryId");

            // ✅ **Thêm danh sách remainingImages vào request**, nếu mà >1 thì tự động biến thành List
            foreach (var image in remainingImages)
            {
                content.Add(new StringContent(image), "remainingImages");
            }

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

            var response = await _httpClient.PutAsync($"http://localhost:5036/products/{Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update product.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
