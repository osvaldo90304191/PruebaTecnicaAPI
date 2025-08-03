using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PruebaTecnicaAPI.Models;
using PruebaTecnicaAPI.Helpers;

namespace PruebaTecnicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Simul0 una bd
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Description = "Laptop de alto rendimiento", Price = 1500.00m },
            new Product { Id = 2, Name = "Mouse", Description = "Mouse ergonómico", Price = 25.50m }
        };
        // Un ID para simular el auto-incremento.
        private static int _nextId = 3;

        // POST /api/products
        // Endpoint para crear un nuevo producto. Protegido con autenticación.
        [HttpPost]
        [Authorize]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("El producto no puede ser nulo.");
            }

            product.Id = _nextId++;

            // Cifro la descripción del producto antes de guardarla.
            product.Description = AesHelper.Encrypt(product.Description);

            _products.Add(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // GET /api/products/{id}
        // Endpoint para obtener un producto por su ID. Protegido con autenticación.
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetProductById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Descifro  la descripción antes de devolver el producto al cliente.
            product.Description = AesHelper.Decrypt(product.Description);
            
            return Ok(product);
        }

        // PUT /api/products/{id}
        // Endpoint para actualizar un producto. Protegido con autenticación.
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null || updatedProduct.Id != id)
            {
                return BadRequest("ID del producto no coincide.");
            }

            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Actualiza los campos. Cifro la descripción si es necesario.
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Description = AesHelper.Encrypt(updatedProduct.Description);

            return NoContent();
        }

        // DELETE /api/products/{id}
        // Endpoint para eliminar un producto. Protegido con autenticación.
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _products.Remove(product);
            return NoContent();
        }

        // GET /api/products
        // Endpoint para obtener todos los productos. Protegido con autenticación.
        [HttpGet]
        [Authorize]
        public IActionResult GetAllProducts()
        {
            // Descifro todas las descripciones antes de devolver la lista completa.
            var productsWithDecryptedDescriptions = _products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = AesHelper.Decrypt(p.Description)
            }).ToList();

            return Ok(productsWithDecryptedDescriptions);
        }
    }
}