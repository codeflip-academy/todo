using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Guids;
using Todo.Domain;
using System.Collections.Generic;
using Todo.Infrastructure.PaymentMethods;

namespace Todo.Infrastructure.EFRepositories
{
    public class EFPaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly TodoDatabaseContext _context;

        public EFPaymentMethodRepository(TodoDatabaseContext context)
        {
            _context = context;
        }
        public void Add(Payment paymentMethod)
        {
            _context.PaymentMethods.Add(paymentMethod);
        }

        public async Task<Payment> FindByAccountIdAsync(Guid Id)
        {
            return await _context.PaymentMethods.Where(x => x.AccountId == Id).FirstOrDefaultAsync();
        }

        public async Task<Payment> FindByIdAsync(string Id)
        {
            return await _context.PaymentMethods.Where(x => x.TokenId == Id).FirstOrDefaultAsync();
        }

        public Task<Payment> FindByTokenIdAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public Guid NextId()
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
             return await _context.SaveChangesAsync();
        }
    }
}