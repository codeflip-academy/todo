using System;
using System.Threading.Tasks;
using Todo.Core;

namespace Todo.Domain.Repositories
{
    public interface ICouponRepository : IUnitOfWork
    {
        Task<Coupon> GetCouponByIdAsync(int Id);
    }
}