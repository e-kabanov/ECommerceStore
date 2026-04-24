using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerceStore.DTOs
{
    public class UpdateOrderStatusModel
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public List<SelectListItem>? OrderStatusList { get; set; }
    }
}
