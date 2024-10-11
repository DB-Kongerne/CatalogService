using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Diagnostics;

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

        [HttpGet("getproduct/{productId}", Name = "GetProductById")]
        public Product Get(Guid productId)
        {
            _logger.LogInformation("Metode GetProduct called at {DT}",
            DateTime.UtcNow.ToLongTimeString());
            return _products.FirstOrDefault(p => p.Id == productId);
        }


        [HttpGet("version")]
        public async Task<Dictionary<string, string>> GetVersion()
        {
            var properties = new Dictionary<string, string>();
            var assembly = typeof(Program).Assembly;
            properties.Add("service", "qgt-customer-service");
            var ver = FileVersionInfo.GetVersionInfo(typeof(Program)
            .Assembly.Location).ProductVersion;
            properties.Add("version", ver!);
            try
            {
                var hostName = System.Net.Dns.GetHostName();
                var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
                var ipa = ips.First().MapToIPv4().ToString();
                properties.Add("hosted-at-address", ipa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                properties.Add("hosted-at-address", "Could not resolve IP-address");
            }
            return properties;
        }

        // POST method to receive product object and add a product
        [HttpPost("add-product")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            _logger.LogInformation("Metode add-product called at {DT}",
            DateTime.UtcNow.ToLongTimeString());

            // Tjek om produktet allerede findes baseret på ProductID
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);

            if (existingProduct != null)
            {
                // Log en advarsel hvis produktet allerede findes
                _logger.LogWarning("Attempt to add product with ID {ProductID} which already exists at {DT}", product.Id, DateTime.UtcNow.ToLongTimeString());

                // Returner en HTTP statuskode 409 (Conflict)
                return StatusCode(StatusCodes.Status409Conflict, "Product with this ID already exists.");
            }

            // Tilføj produktet til listen, hvis det ikke findes
            _products.Add(product);

            // Log information om tilføjelse af produktet
            _logger.LogInformation("New product with ID {ProductID} added successfully at {DT}", product.Id, DateTime.UtcNow.ToLongTimeString());

            // Returner en HTTP statuskode 201 (Created)
            return StatusCode(StatusCodes.Status201Created, "Product added successfully.");
        }
    }
}
