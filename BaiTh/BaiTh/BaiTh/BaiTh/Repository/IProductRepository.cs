using BaiTh.Models;
using Microsoft.EntityFrameworkCore;

namespace BaiTh.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> GetDiscountedProductsAsync();
        Task<Coupon> GetCouponByProductIdAsync(int productId);
    }
}
