using System.ComponentModel.DataAnnotations;

namespace WebCosmeticsStore.Models
{
    public class Product
    {
        [Required, StringLength(10)]
        public string ProductId { get; set; }
        [Required, StringLength(100)] 
        public required string Name { get; set; }
        [Range(0.01, 1000000.000)]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public List<ProductImage>? Images { get; set; }
        public string? ImageUrl {  get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<CartItem>? CartItems { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
