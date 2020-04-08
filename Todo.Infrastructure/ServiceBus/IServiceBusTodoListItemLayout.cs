using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Infrastructure.ServiceBus
{
    public interface IServiceBusTodoListItemLayout
    {
        void SendServiceBusTodoListItemLayout();
        void ReceiveServiceBusTodoListItemLayout();
    }
}
