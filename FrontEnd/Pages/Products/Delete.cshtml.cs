using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEnd.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public Guid Id { get; set; }

        public async Task<IActionResult> OnGet(Guid id)
        {
            Id = id;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:5036/products/{Id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to delete product.");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
