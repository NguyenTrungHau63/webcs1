using Microsoft.EntityFrameworkCore;
using System.Numerics;
using WebCosmeticsStore.Models;
using X.PagedList;

namespace WebCosmeticsStore.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Products
            .Include(p => p.Category) 
            .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAllAndPageAsync(int page = 1)
        {
            int pagesize =20;
            // return await _context.Products.ToListAsync();
            return await _context.Products
            .Include(p => p.Category)
            .ToPagedListAsync(page, pagesize);
        }
        public async Task<Product> GetByIdAsync(string productId)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == productId);
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(string productId)
        {
            var product = await _context.Products.FindAsync(productId);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
