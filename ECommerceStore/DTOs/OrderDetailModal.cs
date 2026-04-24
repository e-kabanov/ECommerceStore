using ECommerceStore.Models;

namespace ECommerceStore.DTOs
{
    public class OrderDetailModal
    {
        public string DivId { get; set; }
        public List<OrderItem> OrderDetails { get; set; }
    }
}
