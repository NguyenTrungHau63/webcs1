using System.ComponentModel.DataAnnotations;

namespace WebCosmeticsStore.Models
{
    public class OrderDetail
    {
        public string  ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
