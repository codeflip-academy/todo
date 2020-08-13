using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Todo.Core;
using Todo.Domain;
using Todo.Domain.Repositories;

namespace Todo.Infrastructure.EFRepositories
{
    public class EFSubItemRepository : ISubItemRepository
    {
        private readonly TodoDatabaseContext _context;
        private readonly ISequentialIdGenerator _idGenerator;

        public EFSubItemRepository(TodoDatabaseContext context, ISequentialIdGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        public void Add(SubItem subItem)
        {
            _context.SubItems.Add(subItem);
        }

        public async Task<List<SubItem>> FindAllSubItemsByListItemIdAsync(Guid listItemId)
        {
            return await _context.SubItems.Where(x => x.ListItemId == listItemId).ToListAsync();
        }

        public async Task<SubItem> FindByIdAsync(Guid subItemId)
        {
            return await _context.SubItems.FindAsync(subItemId);
        }

        public async Task<SubItem> FindByListItemId(Guid listItemId)
        {
            return await _context.SubItems.FindAsync(listItemId);
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
