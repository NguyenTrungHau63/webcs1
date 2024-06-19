using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCosmeticsStore.Models;

namespace WebCosmeticsStore.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAllAndPageAsync(int page = 1);
        Task<Product> GetByIdAsync(string id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(string id);
    }
}
