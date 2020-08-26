using System;
using System.Threading.Tasks;
using Todo.Core;

namespace Todo.Domain.Repositories
{
    public interface IDiscountRepository : IUnitOfWork
    {
        Task<Discount> GetDiscountByNameAsync(string discountName);
        Task<Discount> GetDiscountByIdAsync(int discountId);
    }
}