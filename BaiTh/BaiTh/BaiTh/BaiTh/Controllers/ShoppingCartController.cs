using BaiTh.Data;
using BaiTh.Extensions;
using BaiTh.Models;
using BaiTh.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Baith.Helpers;
using Baith.Services;
using Baith.ViewModels;
namespace BaiTh.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVnPayService _vnPayservice;
        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository
       productRepository, IVnPayService vnPayservice)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _vnPayservice = vnPayservice;
        }
        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string payment = "COD")

        {
            var cart =
           HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Xử lý giỏ hàng trống...
                return RedirectToAction("Index");
            }

            if(payment =="Thanh toán VNPay"){
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = (double)cart.Items.Sum(item => item.Price * item.Quantity) ,
                    FullName = "Nguyễn Văn A", // Tên của người mua
                    CreatedDate = DateTime.Now, // Thời gian hiện tại
                    Description = "123 Đường ABC, Quận XYZ, Thành phố HCM", // Địa chỉ giao hàng
                    OrderId = new Random().Next(1000, 100000), // Mã đơn hàng ngẫu nhiên
                };
                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return View("OrderCompleted", order.Id); // Trang xác nhận hoàn 

        }
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                var coupon = await _productRepository.GetCouponByProductIdAsync(productId);
                decimal discountedPrice = product.Price;
                if (coupon != null)
                {
                    // Kiểm tra nếu phiếu giảm giá hết hạn
                    if (coupon.ValidUntil < DateTime.Now)
                    {
                        // Nếu phiếu giảm giá hết hạn, sử dụng giá gốc
                        discountedPrice = product.Price;
                    }
                    else
                    {
                        // Nếu phiếu giảm giá chưa hết hạn, áp dụng giảm giá
                        discountedPrice *= 1 - (coupon.DiscountPercentage / 100);
                    }
                }

                // Lấy URL hình ảnh đầu tiên hoặc một giá trị mặc định nếu không có hình ảnh
                var imageUrl = product.ImageUrl ?? "/images/default-product-image.png";

                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Name = product.Name,
                    Price = discountedPrice, // Sử dụng giá đã giảm sau khi áp dụng phiếu giảm giá hoặc giá gốc nếu phiếu hết hạn
                    Quantity = quantity,
                    ImageUrl = imageUrl // Đã thêm ImageUrl vào CartItem
                };

                // Lấy giỏ hàng hiện tại từ session hoặc tạo mới nếu chưa có
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

                // Thêm sản phẩm vào giỏ hàng
                cart.AddItem(cartItem);

                // Lưu giỏ hàng cập nhật vào session
                HttpContext.Session.SetObjectAsJson("Cart", cart);

                // Quay trở lại trang danh sách giỏ hàng
                return RedirectToAction("Index");
                // Note: Bạn có thể thay đổi "Index" thành bất kỳ hành động/controller nào bạn muốn người dùng quay lại sau khi thêm sản phẩm.
            }
            else
            {
                return NotFound();
            }
        }


        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }
        // Các actions khác...
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId);

                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.UpdateQuantity(productId, quantity);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");

        }
     
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }


            // Lưu đơn hàng vô database

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
        public IActionResult Success()
        {
            return View();
        }
    }
}