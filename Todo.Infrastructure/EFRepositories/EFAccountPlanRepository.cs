using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using Todo.Domain.Repositories;
using Todo.Domain;
using Todo.Core;

namespace Todo.Infrastructure.EFRepositories
{
    public class EFAccountPlanRepository : IAccountPlanRepository
    {
        private readonly TodoDatabaseContext _context;
        private readonly ISequentialIdGenerator _sequentialIdGenerator;

        public EFAccountPlanRepository(TodoDatabaseContext context, ISequentialIdGenerator sequentialIdGenerator)
        {
            _context = context;
            _sequentialIdGenerator = sequentialIdGenerator;
        }

        public Task AddAccountPlanAsync(AccountPlan accountPlan)
        {
            _context.AccountsPlans.Add(accountPlan);

            return Task.CompletedTask;
        }

        public async Task<AccountPlan> FindAccountPlanByAccountIdAsync(Guid accountId)
        {
            return await _context.AccountsPlans.Where(x => x.AccountId == accountId).FirstOrDefaultAsync();
        }

        public Guid NextId()
        {
            return _sequentialIdGenerator.NextId();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}