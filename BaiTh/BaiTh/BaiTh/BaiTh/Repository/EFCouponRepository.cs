using BaiTh.Data;
using BaiTh.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaiTh.Repository
{
	public class EFCouponRepository : ICouponRepository
	{
		private readonly ApplicationDbContext _context;
        public EFCouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Coupon>> GetAllAsync()
        {
            return await _context.Coupons.ToListAsync();
        }


        public async Task<Coupon> GetByIdAsync(int id)
        {
            return await _context.Coupons.FindAsync(id);
        }

        public async Task AddAsync(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task<Coupon> GetCouponByProductIdAsync(int productId)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.Products.Any(p => p.Id == productId));
        }
        // Trong lớp EFCouponRepository
        public async Task<List<Product>> GetProductsByCouponIdAsync(int couponId)
        {
            // Sử dụng context để truy vấn cơ sở dữ liệu và lấy danh sách các sản phẩm liên quan đến một phiếu giảm giá cụ thể
            return await _context.Products.Where(p => p.CouponId == couponId).ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetAllCouponsByProductIdAsync(int productId)
        {
            return await _context.Coupons
                .Where(c => c.Products.Any(p => p.Id == productId))
                .ToListAsync();
        }
        public async Task UpdateExpiredCouponsAsync()
        {
            // Lấy danh sách các phiếu giảm giá đã hết hạn
            var expiredCoupons = await _context.Coupons.Where(c => c.ValidUntil < DateTime.Now).ToListAsync();

            // Duyệt qua từng phiếu giảm giá đã hết hạn và cập nhật vào cơ sở dữ liệu
            foreach (var coupon in expiredCoupons)
            {
                coupon.IsActive = false; // Tùy thuộc vào yêu cầu cụ thể, bạn có thể đặt trạng thái của phiếu giảm giá thành false hoặc làm điều gì đó khác.
                await _context.SaveChangesAsync();
            }
        }


    }
}
