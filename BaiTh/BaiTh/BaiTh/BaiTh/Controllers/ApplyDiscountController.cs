using BaiTh.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaiTh.Models;
using System;
using System.Linq;

namespace BaiTh.Controllers
{
	public class ApplyDiscountController : Controller
	{
		private readonly ApplicationDbContext _dbContext;

		// Constructor that injects the ApplicationDbContext into the controller
		public ApplyDiscountController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;  // Assign the injected context to your private field
		}

		public IActionResult Index()
		{
			// Now you can use _dbContext here
			var products = _dbContext.Products.ToList();
			return View(products);
		}

		// Giả sử bạn đã có một action để xem chi tiết sản phẩm và áp dụng giảm giá
		[HttpPost]
		public IActionResult ApplyDiscount(int productId, string code)
		{
			var product = _dbContext.Products.Find(productId);
			if (product == null)
			{
				return NotFound();
			}

			var coupon = _dbContext.Coupons
				.FirstOrDefault(c => c.Code == code && c.ValidFrom <= DateTime.Now && c.ValidUntil >= DateTime.Now);

			if (coupon == null)
			{
				ViewBag.Error = "Mã giảm giá không hợp lệ hoặc đã hết hạn.";
				return View(product);
			}

			product.Price = product.CalculateDiscountedPrice(coupon.DiscountPercentage);
			_dbContext.SaveChanges();

			return RedirectToAction("ProductDetails", new { id = product.Id });
		}


		private decimal CalculateDiscountedPrice(decimal originalPrice, double discountPercentage)
		{
			return originalPrice - (originalPrice * (decimal)(discountPercentage / 100));
		}
	}
}
