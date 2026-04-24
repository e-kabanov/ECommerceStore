using ECommerceStore.Data;
using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Task<Product?> GetProductById(int id);
        Task<Product> AddProduct(Product product);
        Task<Product?> UpdateProduct(Product product);
        Task<bool> DeleteProduct(Product product);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDatabaseContext _context;
        public ProductRepository(EcommerceDatabaseContext context) 
        {
            _context = context;
        }

        public async Task<Product> AddProduct(Product product)
        {
            product.CreatedAt = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            //var product = await _context.Products.FindAsync(id);
            //if (product == null) { return false; }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;


        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);

            if (existingProduct == null) { return null; }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            

            if (product.Photo != existingProduct.Photo )
            {
                existingProduct.Photo = product.Photo;
            }

            await _context.SaveChangesAsync();
            return existingProduct;
        }
    }
}
