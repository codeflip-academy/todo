using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Todo.Core
{
    // Copyright (c) .NET Foundation. All rights reserved.
    // Copyright (c) .NET Foundation. All rights reserved.
    // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

    // https://github.com/dotnet/efcore/blob/master/src/EFCore/ValueGeneration/SequentialGuidValueGenerator.cs
    public class SequentialIdGenerator : ISequentialIdGenerator
    {
        private long _counter = DateTime.UtcNow.Ticks;
        public Guid NextId()
        {
            var guidBytes = Guid.NewGuid().ToByteArray();
            var counterBytes = BitConverter.GetBytes(Interlocked.Increment(ref _counter));
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(counterBytes);
            guidBytes[08] = counterBytes[1];
            guidBytes[09] = counterBytes[0];
            guidBytes[10] = counterBytes[7];
            guidBytes[11] = counterBytes[6];
            guidBytes[12] = counterBytes[5];
            guidBytes[13] = counterBytes[4];
            guidBytes[14] = counterBytes[3];
            guidBytes[15] = counterBytes[2];
            return new Guid(guidBytes);
        }
    }
}
