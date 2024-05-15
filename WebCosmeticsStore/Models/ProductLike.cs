using System.ComponentModel.DataAnnotations;

namespace WebCosmeticsStore.Models
{
    public class ProductLike
    {
        [Required, StringLength(10)]
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
