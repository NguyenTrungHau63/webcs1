using Microsoft.EntityFrameworkCore;
using WebCosmeticsStore.Models;
using WebCosmeticsStore.Repositories;

namespace WebCosmeticsStore.Repositories
{
    public class EFShoppingCart : IShoppingCart
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;

        public EFShoppingCart(ApplicationDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }      
        public async Task<List<Product>> GetCartItemsAsync(string userId)
        {
            // Lấy các mục trong giỏ hàng của người dùng cụ thể
            var cartItems = await _context.CartItems
                .Where(item => item.UserId == userId)
                .Include(item => item.Product) // Nếu có một liên kết với bảng sản phẩm
                .ToListAsync();
            var products = cartItems.Select(item => item.Product).ToList();

            return products;
        }
        public async Task UpdateCartItemQuantityAsync(string userId, string productId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> RemoveFromCartAsync(string userId, string productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                return await _context.SaveChangesAsync();
            }

            return 0;
        }
    }
}
