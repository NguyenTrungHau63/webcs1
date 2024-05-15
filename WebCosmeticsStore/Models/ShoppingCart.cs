using System.ComponentModel.DataAnnotations;

namespace WebCosmeticsStore.Models
{
    public class ShoppingCart
    {
        public Product Product { get; set; }
        [Range(0, 10000)]
        public int Count { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(string productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }
        public void UpdateItem(string productId, int quantity)
        {
            Items.FirstOrDefault(p => p.ProductId == productId).Quantity = quantity;
        }
    }
}
