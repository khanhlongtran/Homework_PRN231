using Homework2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    Id = p.Id,
                    PName = p.PName,
                    PPrice = p.PPrice,
                    PQuantity = p.PQuantity,
                    Images = p.Images,  // mỗi product cũng sẽ có 1 list các ảnh
                    CategoryName = p.Category != null ? p.Category.CatName : null
                })
                .ToListAsync();

            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    Id = p.Id,
                    PName = p.PName,
                    PPrice = p.PPrice,
                    PQuantity = p.PQuantity,
                    Images = p.Images,  // Chứa 1 list các ảnh
                    CategoryName = p.Category != null ? p.Category.CatName : null
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] string pName,
                                                      [FromForm] int pPrice,
                                                      [FromForm] int pQuantity,
                                                      [FromForm] Guid categoryId,
                                                      [FromForm] List<IFormFile>? imageFiles)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return BadRequest("Category not found");

            List<string> imagePaths = new List<string>(); // Danh sách lưu các ảnh
            if (imageFiles != null && imageFiles.Count > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploads); // Tạo thư mục nếu chưa có

                foreach (var imageFile in imageFiles)
                {
                    var imagePath = Path.Combine(uploads, imageFile.FileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imagePaths.Add(imageFile.FileName); // Chỉ lưu tên file
                }
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                PName = pName,
                PPrice = pPrice,
                PQuantity = pQuantity,
                Images = string.Join(";", imagePaths), // Ghép các tên file thành chuỗi
                Category = category
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id,
                                             [FromForm] string pName,
                                             [FromForm] int pPrice,
                                             [FromForm] int pQuantity,
                                             [FromForm] Guid categoryId,
                                              [FromForm] List<string>? remainingImages, // Danh sách ảnh còn lại,
                                              [FromForm] List<IFormFile>? imageFiles)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return BadRequest("Category not found");

            // Thư mục lưu ảnh
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(uploads);

            List<string> newImagePaths = new List<string>();

            // Nếu có ảnh mới, lưu lại
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    var imagePath = Path.Combine(uploads, imageFile.FileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    newImagePaths.Add(imageFile.FileName);
                }
            }

            // Ảnh cuối cùng = ảnh còn lại + ảnh mới
            List<string> finalImages = new List<string>();
            if (remainingImages != null)
            {
                finalImages.AddRange(remainingImages);
            }
            finalImages.AddRange(newImagePaths);

            // Cập nhật danh sách ảnh
            product.Images = string.Join(";", finalImages);
            product.PName = pName;
            product.PPrice = pPrice;
            product.PQuantity = pQuantity;
            product.Category = category;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("/categories")]
        public async Task<ActionResult<IEnumerable<object>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    Id = c.Id,
                    CatName = c.CatName
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("images/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var imageFileStream = System.IO.File.OpenRead(filePath);
            return File(imageFileStream, "image/jpeg");
        }

    }
}
