using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Core;

namespace Todo.Domain
{
    public interface IDowngradeRepository : IUnitOfWork
    {
        Task Add(Guid accountId, DateTime billingCycleEnd, int planId);
        Task<Downgrade> GetDowngradeByAccountIdAsync(Guid accountId);
        Task<List<Downgrade>> GetDowngradesAsync();
        Task Remove(Downgrade downgrade);
    }
}