using System;
using System.Threading.Tasks;
using Todo.Core;

namespace Todo.Domain
{
    public interface IDowngradeRepository : IUnitOfWork
    {
        Task Add(Guid accountId, DateTime? billingCycleEnd, int planId);
    }
}