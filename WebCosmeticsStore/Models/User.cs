using Microsoft.AspNetCore.Identity;

namespace WebCosmeticsStore.Models
{
    public class User : IdentityUser
    {
        public string FullName {  get; set; }
        public string Address { get; set; }

        public List<CartItem>? CartItem { get; set; }
        public List<Order>? Order { get; set; }
    }
}
