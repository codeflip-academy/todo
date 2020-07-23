using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Domain.Repositories
{
    public interface IAccountPlanRepository : IRepository
    {
        Task<AccountPlan> FindAccountPlanByAccountIdAsync(Guid accountId);
        Task AddAccountPlanAsync(AccountPlan accountPlan);
    }
}
