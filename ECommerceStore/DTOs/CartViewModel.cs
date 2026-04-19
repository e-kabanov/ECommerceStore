namespace ECommerceStore.DTOs
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
