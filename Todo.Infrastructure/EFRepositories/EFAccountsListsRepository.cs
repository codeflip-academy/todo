using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Guids;
using Todo.Domain;

namespace Todo.Infrastructure.EFRepositories
{
    public class EFAccountsListsRepository : IAccountsListsRepository
    {
        private readonly TodoDatabaseContext _context;
        private readonly ISequentialIdGenerator _sequentialIdGenerator;

        public EFAccountsListsRepository(TodoDatabaseContext context, ISequentialIdGenerator sequentialIdGenerator)
        {
            _context = context;
            _sequentialIdGenerator = sequentialIdGenerator;
        }

        public Task AddAccountsListsInvitedAsync(Guid inviteeId, Guid listId)
        {
            var accountsLists = new RoleInvited()
            {
                Id = NextId(),
                AccountId = inviteeId,
                ListId = listId
            };
            accountsLists.Invited();

            _context.AccountsListsInvited.Add(accountsLists);
            return Task.CompletedTask;
        }

        public async Task<AccountsLists> FindAccountsListsByAccountIdAsync(Guid accountId, Guid listId)
        {
            return await _context.AccountsLists
                .Where(a => a.AccountId == accountId && a.ListId == listId)
                .FirstOrDefaultAsync();
        }

        public async Task<RoleContributor> FindAccountsListsContributorByAccountIdAsync(Guid accountId, Guid listId)
        {
            return await _context.AccountsListsContributor
                .Where(a => a.AccountId == accountId && a.ListId == listId)
                .FirstOrDefaultAsync();
        }

        public async Task<RoleDecline> FindAccountsListsDeclinedByAccountIdAsync(Guid accountId, Guid listId)
        {
            return await _context.AccountsListsDeclined
                .Where(a => a.AccountId == accountId && a.ListId == listId)
                .FirstOrDefaultAsync();
        }

        public async Task<RoleInvited> FindAccountsListsInvitedByAccountIdAsync(Guid accountId, Guid listId)
        {
            return await _context.AccountsListsInvited
                .Where(a => a.AccountId == accountId)
                .FirstOrDefaultAsync();
        }

        public async Task<RoleLeft> FindAccountsListsLeftByAccountIdAsync(Guid accountId, Guid listId)
        {
            return await _context.AccountsListsLeft
                .Where(a => a.AccountId == accountId && a.ListId == listId)
                .FirstOrDefaultAsync();
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