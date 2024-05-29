using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaiTh.Data;
using BaiTh.Models;
using BaiTh.Repository;
using System.Diagnostics;

namespace BaiTh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductManagerController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ApplicationDbContext _context;

        public ProductManagerController(IProductRepository productRepository, ICategoryRepository categoryRepository, ICouponRepository couponRepository , ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _couponRepository = couponRepository;
            _context = context;
        }


        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            foreach (var product in products)
            {
                product.Coupon = await _couponRepository.GetByIdAsync(product.CouponId ?? 0);

                // Kiểm tra nếu sản phẩm có phiếu giảm giá và phiếu đã hết hạn
                if (product.Coupon != null && product.Coupon.ValidUntil < DateTime.Now)
                {
                    // Đặt Id của phiếu giảm giá trong sản phẩm thành null
                    product.CouponId = null;
                }
            }
            return View(products);
        }

        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var coupons = await _couponRepository.GetAllAsync(); // Truy vấn tất cả các phiếu giảm giá
            ViewBag.Coupons = new SelectList(coupons, "Id", "Code");
            return View();

        }


        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                if (images != null)
                {
                    product.Images = new List<ProductImage>();
                    foreach (var item in images)
                    {
                        ProductImage image = new ProductImage()
                        {
                            ProductId = product.Id,
                            Url = await SaveImage(item)
                        };
                        product.Images.Add(image);
                    }
                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            var coupons = await _couponRepository.GetAllAsync(); // Truy vấn tất cả các phiếu giảm giá
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Coupons = new SelectList(coupons, "Id", "Code"); // Chuyển danh sách phiếu giảm giá sang ViewBag
            return View();
        }


        // Viết thêm hàm SaveImage (tham khảo bào 02)
        private async Task<string> SaveImage(IFormFile imageUrl)
        {
            var savePath = Path.Combine("wwwroot/images", imageUrl.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await imageUrl.CopyToAsync(fileStream);
            }
            return "/images/" + imageUrl.FileName;
        }


        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.lsImage = _context.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(product);
        }
        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            var coupons = await _couponRepository.GetAllAsync();
            var appliedCouponId = product.CouponId;

            var couponList = coupons.Select(coupon =>
                new SelectListItem
                {
                    Value = coupon.Id.ToString(),
                    Text = coupon.Code,
                    Selected = coupon.Id == appliedCouponId
                }).ToList();

            ViewBag.Coupons = couponList;

            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới và cập nhật URL ảnh sản phẩm
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                // Cập nhật ID của phiếu giảm giá
                existingProduct.CouponId = product.CouponId;

                // Thực hiện cập nhật thông tin sản phẩm vào cơ sở dữ liệu
                await _productRepository.UpdateAsync(existingProduct);

                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            var initialCoupons = await _couponRepository.GetAllAsync(); // Truy vấn danh sách phiếu giảm giá
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.Coupons = new SelectList(initialCoupons, "Id", "Code", product.CouponId); // Chuyển danh sách phiếu giảm giá sang ViewBag
            return View(product);
        }



        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ApplyCouponToProduct(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                var coupons = await _couponRepository.GetAllAsync();
                ViewBag.ProductId = productId;
                ViewBag.Coupons = new SelectList(coupons, "Id", "Code");
                return View();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCouponToProduct(int productId, int couponId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                var coupons = await _couponRepository.GetAllAsync();
                product.CouponId = couponId;

                // Thêm dòng này để theo dõi giá trị của productId và couponId

                await _productRepository.UpdateAsync(product);
                ViewBag.Coupons = new SelectList(coupons, "Id", "Code");
                return RedirectToAction("Index");
            }
            return NotFound();
        }
       

    }
}
