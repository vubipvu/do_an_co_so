using BaiTh.Data;
using BaiTh.Models;
using Microsoft.EntityFrameworkCore;

namespace BaiTh.Repository
{
    public class EFOrderRepository :IOrderRepositorycs
    {
        private readonly ApplicationDbContext _context;
        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }
    }
}
