using System;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task Add(Guid accountId, DateTime? billingCycleEnd, int planId)
        {
            _context.Add(new Downgrade
            {
                AccountId = accountId,
                BillingCycleEnd = billingCycleEnd,
                PlanId = planId
            });
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}