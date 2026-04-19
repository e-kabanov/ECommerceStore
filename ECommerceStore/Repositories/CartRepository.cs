using ECommerceStore.Data;
using ECommerceStore.DTOs;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace ECommerceStore.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int productId, int qty);
        Task<int> RemoveItem(int itemId);
        Task<List<CartItemViewModel>> GetUserCart();
        Task<int> GetCartCount(int userId);
        Task<int> RemoveItemCompletely(int itemId);

    }

    public class CartRepository : ICartRepository
    {
        private readonly EcommerceDatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(EcommerceDatabaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int productId, int qty)
        {
            string userId = GetUserId();
            int.TryParse(userId, out var userIdInt);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Пользователь не авторизован");
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userIdInt,
                    CreatedAt = DateTime.Now
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartId == cart.CartId && ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += qty;
            }

            else
            {
                var product =  _context.Products.Find(productId);

                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = qty
                };

                _context.CartItems.Add(cartItem);
                
            }

            await _context.SaveChangesAsync();

            return await GetCartCount(userIdInt);

        }

        public async Task<List<CartItemViewModel>> GetUserCart()
        {
            string userId = GetUserId();
            int.TryParse(userId, out var userIdInt);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userIdInt);

            if (cart == null || !cart.CartItems.Any()) { return new List<CartItemViewModel>(); }

            return cart.CartItems.Select(ci => new CartItemViewModel
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductPhoto = ci.Product.Photo,
                ProductDescription = ci.Product.Description,
                Price = ci.Product.Price,
                Quantity = ci.Quantity,
                Stock = ci.Product.Stock
            }).ToList();

        }

        public async Task<int> RemoveItem(int cartItemId)
        {
            string userId = GetUserId();
            int.TryParse(userId, out var userIdInt);

            var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                throw new InvalidOperationException("Товар не найден в корзине");
            }

            else if (cartItem.Quantity == 1)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity -= 1;
                
            }

            await _context.SaveChangesAsync();

            return await GetCartCount(userIdInt);
        }

        public async Task<int> RemoveItemCompletely(int cartItemId)
        {
            string userId = GetUserId();
            if (!int.TryParse(userId, out var userIdInt))
            {
                throw new UnauthorizedAccessException("Неверный формат ID пользователя");
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                throw new InvalidOperationException("Товар не найден в корзине");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return await GetCartCount(userIdInt);
        }


        public async Task<int> GetCartCount(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return 0;
            }

            return cart.CartItems.Sum(ci => ci.Quantity);
        }

        private string GetUserId()
        {

            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Пользователь не авторизован");
            }

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            return userId;   
               
        }

        
    }
}
