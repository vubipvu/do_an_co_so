using BaiTh.Models;
using BaiTh.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaiTh.Controllers
{
	public class HomeController : Controller
	{
	
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;

        public HomeController(IProductRepository productRepository, ICouponRepository couponRepository)
        {
            _productRepository = productRepository;
            _couponRepository = couponRepository;
        }

        


        public async Task<IActionResult> Index()
        {
            var discountedProducts = await _productRepository.GetDiscountedProductsAsync();
            return View(discountedProducts);
        }

        public IActionResult Call()
		{
			return View();
		}
        public IActionResult Privacy()
        {
            return View();
        }
		public IActionResult Trade()
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
