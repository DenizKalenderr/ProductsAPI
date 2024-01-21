using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.DTO;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    // localhost:5000/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController:ControllerBase
    {
        private readonly ProductsContext _contex;

        public ProductsController(ProductsContext context)
        {
            _contex = context;
            
        } 

        // localhost:5000/api/products => GET
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            // İstediğimiz alanları getireceğiz.
            // where ile aktif olan ürünler üzerinde seçim yaparız.
            var products = await _contex
            .Products.Where(i => i.IsActive).Select(p => ProductToDTO(p)).ToListAsync();
            
            return Ok(products);
        }

        // localhost:5000/api/products/id => GET
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var p = await _contex
                            .Products.Where(i => i.ProductId == id)
                            .Select(p => ProductToDTO(p))
                            .FirstOrDefaultAsync();

            if( p == null)
            {
                return NotFound();
            }

            return Ok(p);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            _contex.Products.Add(entity);
            await _contex.SaveChangesAsync();

            //nameof = string ifadeyi yanlış yazdığımızda kod çalışmadan hata vermesini sağlar. Güvenli kod yazmada önemlidir. 
            return CreatedAtAction(nameof(GetProduct), new { id = entity.ProductId }, entity);
        }

        // localhost:5000/api/products/id => PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product entity)
        {
            if(id != entity.ProductId)
            {
                // Kullanıcı tarafından yanlış bir bilgi gönderdin.
                return BadRequest();
            }

            var product = await _contex.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if(product == null)
            {
                return NotFound();
            }

            product.ProductName = entity.ProductName;
            product.Price = entity.Price;
            product.IsActive = entity.IsActive;

            try
            {
                await _contex.SaveChangesAsync();
            }
            catch(Exception)
            {
                return NotFound();
            }

            // Geriye bir değer döndürmeyiz. Güncelleme başarılı durum kodu. 204.
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var product = await _contex.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if(product == null)
            {
                return NotFound();
            }

            _contex.Products.Remove(product);

            try
            {
                await _contex.SaveChangesAsync();
            }
            catch(Exception)
            {
                return NotFound();
            }

            //204
            return NoContent();
        }

        private static ProductDTO ProductToDTO(Product p)
        {
            var entity = new ProductDTO();

            if(p != null)
            {
                entity.ProductId = p.ProductId;
                entity.ProductName = p.ProductName;
                entity.Price = p.Price;
            }
            return entity;
        }
    }
}