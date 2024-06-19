using WebCosmeticsStore.Models;
using X.PagedList;

namespace WebCosmeticsStore.ViewsModels
{
    public class VMHomeP
    {
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
