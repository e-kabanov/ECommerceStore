using ECommerceStore.Models;

namespace ECommerceStore.DTOs
{
    public class ProductDisplayModel
    {
        public IEnumerable<Product> Products { get; set; }
        public string SearchTerm {  get; set; }
    }
}
