using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ILogger<CatalogController> logger)
        {
            _logger = logger;
        }

        private static List<Product> _products = new List<Product>()
        {
            new()
            {
                Id = new Guid("7125e019-c469-4dbd-93e5-426de6652523"),
                Name = "Salmon Fillet",
                Description = "Fresh salmon fillet",
                Price = 12.99m,
                Brand = "FishmongerX",
                Manufacturer = "Fish Supplier",
                Model = "Standard",
                ImageUrl = "https://example.com/salmon.jpg",
                ProductUrl = "https://example.com/salmon",
                ReleaseDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(3)
            }
        };

        [HttpGet("{productId}", Name = "GetProductById")]
        public Product Get(Guid productId)
        {
            return _products.FirstOrDefault(p => p.Id == productId);
        }

        // POST method to receive customer object and add a product
        [HttpPost("add-product")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("product is null.");
            }

            // Add product to the product list (you can adjust logic if it's supposed to add to the customer)
            _products.Add(product);

            return Ok("Product added successfully.");
        }
    }
}
