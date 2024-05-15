using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCosmeticsStore.Models;
using WebCosmeticsStore.Repositories;

namespace WebCosmeticsStore.Controllers
{
    
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository _categoryRepository;
            
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (!ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<ActionResult> Detail(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        //Process the book upadte
        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        //Process the book deletion
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
