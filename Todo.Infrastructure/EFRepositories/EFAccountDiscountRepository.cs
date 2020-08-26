using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Todo.Core;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace Todo.Infrastructure.EFRepositories
{
    public class EFAccountDiscountRepository : IAccountDiscountRepository
    {
        private readonly TodoDatabaseContext _context;
        private readonly ISequentialIdGenerator _idGenerator;

        public EFAccountDiscountRepository(TodoDatabaseContext context, ISequentialIdGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        public async Task AddAsync(AccountDiscount accountDiscount)
        {
            _context.AccountsDiscounts.Add(accountDiscount);
        }

        public async Task<AccountDiscount> GetUnredeemedDiscountByAccountIdAsync(Guid accountId)
        {
            return await _context.AccountsDiscounts
                .Where(x => x.AccountId == accountId && x.AppliedToSubscription == false).FirstOrDefaultAsync();
        }

        public Guid NextId()
        {
            return _idGenerator.NextId();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}