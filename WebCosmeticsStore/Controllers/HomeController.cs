using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using System.Security.Claims;
using WebCosmeticsStore.Models;
using WebCosmeticsStore.Repositories;
using WebCosmeticsStore.ViewsModels;
using X.PagedList;

namespace WebCosmeticsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ApplicationDbContext context)
        {
            _logger = logger;
            _productRepository = productRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(int? id)
        {
            
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            
			if (id.HasValue)
			{
                products = products.Where(x => x.CategoryId == id).ToList();
			}
			var model = new VMHomeC()
            {
                Products = products,
                Categories = categories
            };
            return View(model);
        }

        public async Task<IActionResult> Sort(string sortBy, int page = 1)
        {
            int pageSize = 20;
            IEnumerable<Product> products = HttpContext.Session.GetObjectFromJson<IEnumerable<Product>>("ProductFilter") ?? await _productRepository.GetAllAsync();
            switch (sortBy)
            {
                case "PriceDesc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "PriceAsc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "NameAsc":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "NameDesc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                default:
                    break;
            }

            // Paginate the sorted products
            var pagedProducts = products.ToPagedList(page, pageSize);

            // Load images for each product in the current page
            foreach (var product in pagedProducts)
            {
                product.Images = await _context.ProductImages
                    .Where(pi => pi.ProductId == product.ProductId)
                    .ToListAsync();
            }

            HttpContext.Session.SetObjectAsJson("ProductFilter", products);

            return View("Index", pagedProducts);
        }


        public async Task<IActionResult> Search(string searchTerm, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 20;

            // Get the initial paged list of products
            var products = await _context.Products.Include(p => p.Images).ToPagedListAsync(page, pageSize);

            // Apply search filter if searchTerm is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var name = searchTerm.ToUpper().Trim();
                // Filter products based on the search term
                var filteredProducts = _context.Products
                                               .Include(p => p.Images)
                                               .Where(p => p.Name.ToUpper().Contains(name));

                // Get the paged list of the filtered products
                products = await filteredProducts.ToPagedListAsync(page, pageSize);
            }

            // Return the view with the paged list of products
            return View("Index", products);
        }

        public async Task<IActionResult> Detail(string productId)
        {
            var productDetail = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (productDetail == null)
            {
                return NotFound();
            }
            return View(productDetail);
        }
        [HttpPost]
        public IActionResult ToggleLike(string productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingLike = _context.ProductLike
                .FirstOrDefault(pl => pl.ProductId == productId && pl.UserId == userId);

            if (existingLike == null)
            {
                _context.ProductLike.Add(new ProductLike
                {
                    ProductId = productId,
                    UserId = userId
                });
            }
            else
            {
                _context.ProductLike.Remove(existingLike);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Home"); 
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Comment(string UserID, int Rating, string Comment, string ProductID)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserID);
            var comment = new Comment()
            {
                User = user,
                UserID = user.Id,
                FullName = user.FullName,
                ParentID = 0,
                Rate = Rating,
                CommentDate = DateTime.Now,
                ProductID = ProductID,
                Message = Comment
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}
