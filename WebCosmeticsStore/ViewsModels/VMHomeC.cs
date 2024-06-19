using WebCosmeticsStore.Models;

namespace WebCosmeticsStore.ViewsModels
{
    public class VMHomeC
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
