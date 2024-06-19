using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCosmeticsStore.Models;

namespace WebCosmeticsStore.Controllers.api
{
	[Route("api/[controller]")]
	[ApiController]
	public class WishListApiController : ControllerBase
	{
		private ApplicationDbContext _context;
		private UserManager<User> _userManager;

		public WishListApiController(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpPost]
		public async Task<IActionResult> AddToWishlist([FromForm] string productId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var user = await _userManager.GetUserAsync(User);
			var productLike = await _context.ProductLike
				.Include(u => u.User)
				.Include(p => p.Product)
				.FirstOrDefaultAsync(x => x.ProductId == productId);
			// Check if the entry already exists
			var existingEntry = await _context.ProductLike
				.FirstOrDefaultAsync(pl => productLike.ProductId == productId);

			if (existingEntry != null)
			{
				return BadRequest("Product already in wishlist.");
			}
			productLike = new ProductLike 
			{ 
				ProductId = productId,
				Product = productLike.Product,
				User = user,
				UserId = user.Id
			};
			_context.ProductLike.Add(productLike);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProductLike", new { productId = productLike.ProductId, userId = productLike.UserId }, productLike);
		}
	}
}
