using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Domain.Repositories;

namespace Todo.Domain
{
    public interface IAccountsListsRepository : IRepository
    {
        Task AddAccountsListsInvitedAsync(Guid inviteeId, Guid listId);
        Task<AccountsLists> FindAccountsListsByAccountIdAndListIdAsync(Guid accountId, Guid listId);
        Task<RoleOwner> FindAccountsListsOwnerByAccountIdAndListIdAsync(Guid accountId, Guid listId);
        Task<RoleInvited> FindAccountsListsInvitedByAccountIdAsync(Guid accountId, Guid listId);
        Task<RoleDecline> FindAccountsListsDeclinedByAccountIdAsync(Guid accountId, Guid listId);
        Task<RoleContributor> FindAccountsListsContributorByAccountIdAndListIdAsync(Guid accountId, Guid listId);
        Task<RoleLeft> FindAccountsListsLeftByAccountIdAsync(Guid accountId, Guid listId);
        Task<List<AccountsLists>> FindAccountsListsByAccountIdAsync(Guid accountId);
    }
}