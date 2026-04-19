using ECommerceStore.Data;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Repositories
{

    public interface IHomeRepository
    {
        Task<IEnumerable<Product>> GetProducts(string searchTerm);
    }

    public class HomeRepository : IHomeRepository
    {
        private readonly EcommerceDatabaseContext _context;

        public HomeRepository(EcommerceDatabaseContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetProducts(string searchTerm)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            var products = await productsQuery
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return products;
        } 
    }
}
