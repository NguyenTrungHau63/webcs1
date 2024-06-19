using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebCosmeticsStore.Models;
using WebCosmeticsStore.Repositories;
using X.PagedList;

namespace Areas.Admin.Controllers
{
    [Area("admin")]

    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context, IProductRepository productRepository, IProductImageRepository productImageRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImageRepository;
            _context = context;
        }

        // GET: Product --- Hien thi danh sach san pham
        public async Task<IActionResult> Index(int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pagesize = 10;
            var products = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .ToPagedListAsync(page, pagesize);
            
            return View(products);
        }

        // GET: Product/Details/5  -- Hien thi thong tin chi thiet san pham
        public async Task<IActionResult> Detail(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Images = await _context.ProductImages
                    .Where(pi => pi.ProductId == product.ProductId)
                    .ToListAsync();
            return View(product);
        }

        // GET: Product/Create  -- Hien thi form them san pham moi
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create   -- Xu ly them san pham moi
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.AddAsync(product);
                await _context.SaveChangesAsync(); // Lưu product trước

                if (imageUrl != null)
                {
                    // Lưu đường dẫn hình ảnh đại diện tham khảo hàm SaveImage
                    var imageUrlString = await SaveImage(imageUrl);
                    // Tạo đối tượng ProductImage và lưu vào cơ sở dữ liệu
                    var productImage = new ProductImage { Url = imageUrlString, ProductId = product.ProductId };
                    await _productImageRepository.UpdateAsync(productImage);
                }

                return RedirectToAction(nameof(Index));
            }

            // Xử lý lỗi ModelState
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        // Method SaveImage
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return image.FileName; // Trả về đường dẫn tương đối
        }

        // GET: Product/Update/5  --- Hien thi form cap nhat san pham
        public async Task<IActionResult> Update(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        // POST: Product/Update/5  --- Xu ly cap nhat san pham
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, [Bind("ProductId,Name,Price,Description,CategoryId")] Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl");
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Xử lý ảnh
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    try
                    {
                        // Kiểm tra xem sản phẩm đã có hình ảnh chưa
                        var existingProductImage = await _productImageRepository.GetByProductIdAsync(existingProduct.ProductId);

                        // Nếu đã có, cập nhật lại đường dẫn ảnh
                        if (existingProductImage != null)
                        {
                            existingProductImage.Url = await SaveImage(imageUrl);
                            await _productImageRepository.UpdateAsync(existingProductImage);
                        }
                        else
                        {
                            // Nếu chưa có, tạo mới
                            var imageUrlString = await SaveImage(imageUrl);
                            var productImage = new ProductImage
                            {
                                Url = imageUrlString,
                                ProductId = product.ProductId
                            };
                            await _productImageRepository.AddAsync(productImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ImageUrl", ex.Message);
                        return View(product);
                    }
                }

                // Cập nhật thông tin sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // GET: Product/Delete/5  --- Hien thi form xac nhan xoa san pham
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Images = await _context.ProductImages
                   .Where(pi => pi.ProductId == product.ProductId)
                   .ToListAsync();
            return View(product);
        }

        // POST: Product/Delete/5    --- Xu ly hoa san pham
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
