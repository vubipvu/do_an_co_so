using BaiTh.Models;

namespace BaiTh.Repository
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetAllAsync();
        Task<Coupon> GetByIdAsync(int id);
        Task AddAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task DeleteAsync(int id);
        Task<List<Product>> GetProductsByCouponIdAsync(int couponId);

        Task<Coupon> GetCouponByProductIdAsync(int productId);
    }
}
