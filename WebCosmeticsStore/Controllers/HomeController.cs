using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebCosmeticsStore.Models;
using WebCosmeticsStore.Repositories;

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
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            foreach (var product in products)
            {
                product.Images = await _context.ProductImages
                    .Where(pi => pi.ProductId == product.ProductId)
                    .ToListAsync();
            }
            return View(products);
        }

        public async Task<IActionResult> Sort(string sortBy)
        {
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
            HttpContext.Session.SetObjectAsJson("ProductFilter", products);
            // Load ?nh cho t?ng s?n ph?m
            foreach (var product in products)
            {
                product.Images = await _context.ProductImages
                    .Where(pi => pi.ProductId == product.ProductId)
                    .ToListAsync();
            }
            return View("Index", products);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            IEnumerable<Product> products = HttpContext.Session.GetObjectFromJson<IEnumerable<Product>>("ProductFilter") ?? await _productRepository.GetAllAsync();
            // Áp d?ng tìm ki?m n?u có
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var name = searchTerm.ToUpper().Trim();
                products = products.Where(p => p.Name.ToUpper().Contains(name));
            }
            HttpContext.Session.SetObjectAsJson("ProductFilter", products);

            // Load ?nh cho t?ng s?n ph?m
            foreach (var product in products)
            {
                product.Images = await _context.ProductImages
                    .Where(pi => pi.ProductId == product.ProductId)
                    .ToListAsync();
            }
            return View("Index", products);
        }

        public async Task<IActionResult> Detail(string productId)
        {
            var productDetail = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
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
        
    }
}
