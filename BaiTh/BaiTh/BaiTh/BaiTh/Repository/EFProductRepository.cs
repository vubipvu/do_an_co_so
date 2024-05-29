using BaiTh.Data;
using BaiTh.Models;
using Microsoft.EntityFrameworkCore;

namespace BaiTh.Repository
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<Coupon> GetCouponByProductIdAsync(int productId)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.Products.Any(p => p.Id == productId));
        }

        public async Task<IEnumerable<Coupon>> GetAllCouponsByProductIdAsync(int productId)
        {
            return await _context.Coupons
                .Where(c => c.Products.Any(p => p.Id == productId))
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetDiscountedProductsAsync()
        {
            // Lấy danh sách các sản phẩm có mã giảm giá
            var discountedProducts = await _context.Products
                .Include(p => p.Coupon)
                .Where(p => p.Coupon != null && p.Coupon.DiscountPercentage > 0)
                .ToListAsync();

            return discountedProducts;
        }

    }
}
