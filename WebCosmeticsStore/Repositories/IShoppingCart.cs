using Microsoft.AspNetCore.Mvc;
using WebCosmeticsStore.Models;

namespace WebCosmeticsStore.Repositories
{
    public interface IShoppingCart
    {
        Task<List<Product>> GetCartItemsAsync(string userId);
        Task UpdateCartItemQuantityAsync(string userId, string productId, int quantity);
        Task<int> RemoveFromCartAsync(string userId, string productId);
    }
}
