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
    public class EFAccountRepository : IAccountRepository
    {
        private TodoDatabaseContext _context;
        private readonly ISequentialIdGenerator _idGenerator;

        public EFAccountRepository(TodoDatabaseContext context, ISequentialIdGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);
        }
        public async Task<Account> FindAccountByIdAsync(Guid id) => await _context.Accounts.FindAsync(id);
        public async Task<bool> DoesAccountWithAccountIdExistAsync(Guid accountId) => await _context.Accounts.FindAsync(accountId) != null;
        public async Task RemoveAccountAsync(Guid accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            
            _context.Accounts.Remove(account);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Account> FindAccountByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
        }

        public Guid NextId()
        {
            return _idGenerator.NextId();
        }

        public void UpdateAccountPlan(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void UpdateContributorsAsync(Account account)
        {
            _context.Accounts.Update(account);
        }
    }
}