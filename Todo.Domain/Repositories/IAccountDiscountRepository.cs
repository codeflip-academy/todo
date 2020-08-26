using System;
using System.Threading.Tasks;

namespace Todo.Domain.Repositories
{
    public interface IAccountDiscountRepository : IRepository
    {
        Task AddAsync(AccountDiscount accountDiscount);
        Task<AccountDiscount> GetUnredeemedDiscountByAccountIdAsync(Guid accountId);
    }
}
