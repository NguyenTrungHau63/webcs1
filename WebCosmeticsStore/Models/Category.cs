using System.ComponentModel.DataAnnotations;

namespace WebCosmeticsStore.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public List<Product> Products { get; set; }

    }
}
