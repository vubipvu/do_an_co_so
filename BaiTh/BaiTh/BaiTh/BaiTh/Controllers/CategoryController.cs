using BaiTh.Models;
using BaiTh.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaiTh.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách danh mục
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // Hiển thị form thêm danh mục mới
        //public IActionResult Add()
        //{
        //    return View();
        //}

        //// Xử lý thêm danh mục mới
        //[HttpPost]
        //public async Task<IActionResult> Add(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _categoryRepository.AddAsync(category);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}

        //// Hiển thị form cập nhật danh mục
        //public async Task<IActionResult> Update(int id)
        //{
        //    var category = await _categoryRepository.GetByIdAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}

        //// Xử lý cập nhật danh mục
        //[HttpPost]
        //public async Task<IActionResult> Update(int id, Category category)
        //{
        //    if (id != category.Id)
        //    {
        //        return NotFound();
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        await _categoryRepository.UpdateAsync(category);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}
        public async Task<IActionResult> Display(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //// Hiển thị form xác nhận xóa danh mục
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var category = await _categoryRepository.GetByIdAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}

        //// Xử lý xóa danh mục
        //[HttpPost, ActionName("Delete")]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    await _categoryRepository.DeleteAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
