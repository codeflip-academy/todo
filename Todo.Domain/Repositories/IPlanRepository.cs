using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Domain.Repositories
{
    public interface IPlanRepository : IUnitOfWork
    {
        Task<Plan> FindPlanByIdAsync(int planId);
    }
}
