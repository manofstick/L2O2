using System;
using System.Collections.Generic;

namespace L2O2.Core
{
    interface IConsumableSeq<T>
        : IEnumerable<T> 
    {
        IConsumableSeq<U> Transform<U>(ISeqTransform<T, U> transform);
        Result Consume<Result>(Func<PipeIdx,SeqConsumer<T,Result>> getConsumer);
    }
}
