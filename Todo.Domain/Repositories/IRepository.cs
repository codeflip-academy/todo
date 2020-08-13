using System;
using System.Collections.Generic;
using System.Text;
using Todo.Core;

namespace Todo.Domain.Repositories
{
    public interface IRepository : IUnitOfWork
    {
        Guid NextId();
    }
}
