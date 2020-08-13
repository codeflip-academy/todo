using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using Todo.Domain.Repositories;
using Todo.Domain;

namespace Todo.Infrastructure.EFRepositories
{
    public class EFPlanRepository : IPlanRepository
    {
        private readonly TodoDatabaseContext _context;

        public EFPlanRepository(TodoDatabaseContext context)
        {
            _context = context;
        }
        public async Task<Plan> FindPlanByIdAsync(int planId)
        {
            return await _context.Plans.FindAsync(planId);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}