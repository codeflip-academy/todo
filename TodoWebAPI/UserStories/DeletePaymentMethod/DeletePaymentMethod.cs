using System;
using MediatR;
using Todo.Infrastructure;

namespace TodoWebAPI.UserStories.DeletePaymentMethod
{
    public class DeletePaymentMethod : IRequest
    {
        public Payment Method {get; set;}
    }
}