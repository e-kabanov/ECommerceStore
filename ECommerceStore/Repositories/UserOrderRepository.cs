using ECommerceStore.Data;
using ECommerceStore.DTOs;
using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Repositories
{

    public interface IUserOrderRepository
    {
        Task<List<Order>> AllOrders();
        Task<Order?> GetOrderById(int orderId);
        Task<List<OrderStatus>> GetOrderStatuses();
        Task<bool> UpdateOrderStatus(UpdateOrderStatusModel model);
    }

    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly EcommerceDatabaseContext _context;
        public UserOrderRepository(EcommerceDatabaseContext context)
        {
            _context = context;

        }

        public async Task<List<Order>> AllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders;

        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }

        public async Task<List<OrderStatus>> GetOrderStatuses()
        {
            return await _context.OrderStatuses.ToListAsync();
        }

        public async Task<bool> UpdateOrderStatus(UpdateOrderStatusModel model)
        {
            var order = await _context.Orders.FindAsync(model.OrderId);

            if (order != null)
            {
                order.StatusId = model.OrderStatusId;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
