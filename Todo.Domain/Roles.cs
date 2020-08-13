using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain
{
    public static class Roles
    {
        public readonly static byte Invited = 0;
        public readonly static byte Declined = 1;
        public readonly static byte Contributer = 2;
        public readonly static byte Owner = 3;
        public readonly static byte Left = 4;
    }
}
