using Microsoft.EntityFrameworkCore;
using WebCosmeticsStore.Models;

namespace WebCosmeticsStore.Repositories
{
    public class EFProductImageRepository :  IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductImage productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(ProductImage productImage)
        {
            _context.ProductImages.Update(productImage);
            await _context.SaveChangesAsync();
        }
        public async Task<ProductImage> GetByProductIdAsync(string productId)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(pi => pi.ProductId == productId);
        }

    }
}
