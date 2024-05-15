using System.ComponentModel.DataAnnotations;

namespace WebCosmeticsStore.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public string ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
