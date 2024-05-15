using WebCosmeticsStore.Models;

namespace WebCosmeticsStore.Repositories
{
    public interface IProductImageRepository
    {
        Task AddAsync(ProductImage productImage);
        Task UpdateAsync(ProductImage productImage);
        Task<ProductImage> GetByProductIdAsync(string productId);
    }
}
