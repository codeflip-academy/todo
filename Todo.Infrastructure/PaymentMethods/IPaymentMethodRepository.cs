using System;
using System.Threading.Tasks;
using Todo.Domain.Repositories;

namespace Todo.Infrastructure.PaymentMethods
{
    public interface IPaymentMethodRepository : IRepository
    {
        void Add(Payment paymentMethod);
        Task<Payment> FindByTokenIdAsync(string Id);
        Task<Payment> FindByAccountIdAsync(Guid Id);

        
    }
}