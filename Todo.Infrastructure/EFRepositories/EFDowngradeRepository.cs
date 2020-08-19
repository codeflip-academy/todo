using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Todo.Domain;

namespace Todo.Infrastructure.EFRepositories
{
    public class EFDowngradeRepository : IDowngradeRepository
    {
        private readonly TodoDatabaseContext _context;

        public EFDowngradeRepository(TodoDatabaseContext context)
        {
            _context = context;
        }

        public async Task Add(Guid accountId, DateTime billingCycleEnd, int planId)
        {
            _context.Add(new Downgrade
            {
                AccountId = accountId,
                BillingCycleEnd = billingCycleEnd,
                PlanId = planId
            });
        }

        public async Task<List<Downgrade>> GetDowngradesAsync()
        {
            return _context.Downgrades.Where(x => x.BillingCycleEnd.Date == DateTime.Now.Date).ToList();
        }

        public async Task<Downgrade> GetDowngradeByAccountIdAsync(Guid accountId)
        {
            return await _context.Downgrades.Where(x => x.AccountId == accountId).FirstOrDefaultAsync();
        }

        public async Task Remove(Downgrade downgrade)
        {
            _context.Downgrades.Remove(downgrade);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}