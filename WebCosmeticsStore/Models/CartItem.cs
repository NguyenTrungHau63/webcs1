using System.ComponentModel.DataAnnotations;

namespace WebCosmeticsStore.Models
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public DateTime? Date { get; set; }
        public int Quantity {  get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice => Quantity * UnitPrice;    
        public Product? Product { get; set; }
        public User Users { get; set; }        
    }
}

