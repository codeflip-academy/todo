using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace Todo.Infrastructure
{
    public class EFDiscountRepository : IDiscountRepository
    {
        private readonly TodoDatabaseContext _context;

        public EFDiscountRepository(TodoDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Discount> GetDiscountByNameAsync(string discountName)
        {
            return await _context.Discounts.Where(d => d.Name == discountName).FirstOrDefaultAsync();
        }
        public async Task<Discount> GetDiscountByIdAsync(int discountId)
        {
            return await _context.Discounts.FindAsync(discountId);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}