using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebCosmeticsStore.Models;
using WebCosmeticsStore.Repositories;
using WebCosmeticsStore.Services;


namespace WebCosmeticsStore.Controllers
{
    
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IProductRepository _productRepository;
        private readonly IVnPayService _vnPayService;
        private readonly IShoppingCart _shoppingCart;
        private readonly IProductImageRepository _productImageRepository;

        public ShoppingCartController(ApplicationDbContext context, UserManager<User> userManager, IProductRepository productRepository, IVnPayService vnPayService, IProductImageRepository productImageRepository, IShoppingCart shoppingCart)
        {
            _context = context;
            _userManager = userManager;
            _productRepository = productRepository;
            _vnPayService = vnPayService;
            _shoppingCart = shoppingCart;
            _productImageRepository = productImageRepository;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            if(cart.Items.Count < 1)
            {
                var orders = await _context.Orders
                    .Include(u => u.User)
                    .Include(p => p.OrderDetails)
                        .ThenInclude(p => p.Product)
                        .ThenInclude(p => p.Category)
                    .Where(x => x.User.Id == user.Id)
                    .ToListAsync();
                return View("OrderHistory", orders);
            }
            foreach(var item in cart.Items)
            {
                var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                item.Product = product;
            }

            
            return View(cart);
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var toTalPrice = cart.Items.Sum(i => i.Quantity * i.UnitPrice);
            TempData["TotalPrice"] = toTalPrice;
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string payment = "COD")
        {
            HttpContext.Session.SetObjectAsJson("Order", order);
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var user = await _userManager.GetUserAsync(User);
            if (payment == "Thanh Toán VnPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = cart.Items.Sum(i => (double)i.UnitPrice * i.Quantity),
                    CreatedDate = DateTime.Now,
                    Decription = "Thanh Toán đơn hàng",
                    FullName = user.FullName,
                    OrderId = new Random().Next(1000, 10000)
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => (int)i.UnitPrice * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = (decimal)i.UnitPrice,
            }).ToList();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return View("OrderCompleted", order.Id);
        }
		[Authorize]
		public async Task<IActionResult> AddToCart(string productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            var product = _context.Products
                .FirstOrDefault(p => p.ProductId == productId);
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Thêm sản phẩm vào giỏ hàng
            var cartItem = new CartItem
            {
                UserId = user.Id,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = (double)product.Price,
                Product = product
            };
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index", "ShoppingCart");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveItem(string userId, string productId)
        {
            await _shoppingCart.RemoveFromCartAsync(userId, productId);
            return RedirectToAction("Index");
        }



        private async Task<Product> GetProductFromDatabase(string productId)
        {
            return await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public IActionResult UpdateItem(string productId, int quantity)
         {
             if (string.IsNullOrEmpty(productId) || quantity <= 0)
             {
                 return BadRequest("Invalid productId or quantity");
             }
             var rCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
             rCart.UpdateItem(productId, quantity);
             HttpContext.Session.SetObjectAsJson("Cart", rCart);
             return RedirectToAction("Index");
         }
         public IActionResult PaymentSuccess()
         {
             return View("Success");
         }
         public IActionResult PaymentFail()
         {
             return View("Fail");
         }
         public async Task<IActionResult> PaymentCallBack()
         {
             var response = _vnPayService.PaymentExecute(Request.Query);
             if(response == null || response.VnPayResponseCode != "00") {
                 TempData["Message"] = $"Lỗi thanh toán VnPay : {response.VnPayResponseCode}";
                 return RedirectToAction("PaymentFail");                
             }
             Order order = HttpContext.Session.GetObjectFromJson<Order>("Order");
             var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
             var user = await _userManager.GetUserAsync(User);
             if (cart == null || !cart.Items.Any())
             {
                 return RedirectToAction("Index");
             }

             order.UserId = user.Id;
             order.OrderDate = DateTime.UtcNow;
             order.TotalPrice = cart.Items.Sum(i =>(int) i.UnitPrice * i.Quantity);
             order.OrderDetails = cart.Items.Select(i => new OrderDetail
             {
                 ProductId = i.ProductId,
                 Quantity = i.Quantity,
                 Price =(decimal) i.UnitPrice,
             }).ToList();
             _context.Orders.Add(order);
             await _context.SaveChangesAsync();
             HttpContext.Session.Remove("Cart");

             TempData["Message"] = $"Thanh toán VnPay thành công";
             return RedirectToAction("PaymentSuccess");
         }


        public async Task<IActionResult> DetailOrderHistory(int id)
        {

            return View();
        }
    }
}
