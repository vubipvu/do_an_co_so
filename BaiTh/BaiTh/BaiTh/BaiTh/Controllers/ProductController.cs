using BaiTh.Models;
using BaiTh.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaiTh.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository , ICouponRepository couponRepository, ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _couponRepository = couponRepository;
            _logger = logger;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();

            // Lặp qua danh sách sản phẩm để lấy thông tin coupon của từng sản phẩm
            foreach (var product in products)
            {
                var coupon = await _couponRepository.GetCouponByProductIdAsync(product.Id);
                if (coupon != null)
                {
                    product.Coupon = coupon;
                }
            }

            return View(products);
        }



        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy thông tin về coupon dựa trên ID của sản phẩm
            var coupon = await _couponRepository.GetCouponByProductIdAsync(id);
            // Nếu sản phẩm có coupon, truyền thông tin coupon vào view thông qua ViewBag hoặc model
            if (coupon != null)
            {
                ViewBag.Coupon = coupon;
                // Hoặc có thể truyền thông tin coupon qua model
                // return View("Display", new ProductCouponViewModel { Product = product, Coupon = coupon });
            }

            return View(product);
        }

    }
}
