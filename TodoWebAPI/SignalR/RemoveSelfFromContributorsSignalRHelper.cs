using System;
using System.Collections.Generic;
using Todo.Domain;

namespace Todo.Infrastructure
{
    public static class RemoveSelfFromContributorSignalRHelper
    {
        public static List<string> RemoveContributor(TodoList list, Account account)
        {
            foreach(var contributor in list.Contributors)
            {
                if(contributor == account.Email)
                {
                    list.Contributors.Remove(contributor);
                }
            }
            return list.Contributors;
        }
    }
}