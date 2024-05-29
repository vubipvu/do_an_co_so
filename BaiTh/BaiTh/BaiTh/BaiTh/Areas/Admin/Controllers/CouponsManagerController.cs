using BaiTh.Data;
using BaiTh.Models;
using BaiTh.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaiTh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponsManagerController : Controller
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ApplicationDbContext _context;

        public CouponsManagerController(ICouponRepository couponRepository,ApplicationDbContext context)
        {
            _couponRepository = couponRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _couponRepository.GetAllAsync();
            return View(coupons);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                await _couponRepository.AddAsync(coupon);
                return RedirectToAction(nameof(Index));
            }

            return View(coupon);
        }

        public async Task<IActionResult> Display(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }
       

       
        public async Task<IActionResult> Update(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Coupon coupon)
        {
            ModelState.Remove("ImageUrl");
            if (id != coupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingCoupon = await _couponRepository.GetByIdAsync(id);
                if (existingCoupon == null)
                {
                    return NotFound();
                }

                existingCoupon.Code = coupon.Code;
                existingCoupon.DiscountPercentage = coupon.DiscountPercentage;
                existingCoupon.ValidFrom = coupon.ValidFrom;
                existingCoupon.ValidUntil = coupon.ValidUntil;

                await _couponRepository.UpdateAsync(existingCoupon);
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        // Trong CouponRepository
        public async Task<List<Product>> GetProductsByCouponIdAsync(int couponId)
        {
            return await _context.Products.Where(p => p.CouponId == couponId).ToListAsync();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            await _couponRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
