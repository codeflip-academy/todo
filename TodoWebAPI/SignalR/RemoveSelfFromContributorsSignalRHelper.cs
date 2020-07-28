using System;
using System.Collections.Generic;
using Todo.Domain;

namespace Todo.Infrastructure
{
    public static class RemoveSelfFromContributorSignalRHelper
    {
        public static List<string> RemoveContributor(TodoList list, string accountEmail)
        {
            list.RemoveContribuor(accountEmail);

            return list.Contributors;
        }
    }
}