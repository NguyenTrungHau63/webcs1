using Microsoft.AspNetCore.Mvc;

namespace WebCosmeticsStore.Controllers
{
    public class WishListController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
