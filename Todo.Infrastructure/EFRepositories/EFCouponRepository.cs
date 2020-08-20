using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace Todo.Infrastructure
{
    public class EFCouponRepository : ICouponRepository
    {
        private readonly TodoDatabaseContext _context;

        public EFCouponRepository(TodoDatabaseContext context)
        {
            _context = context;
        }
        public async Task<Coupon> GetCouponByIdAsync(int Id)
        {
            return await _context.Coupons.Where(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}