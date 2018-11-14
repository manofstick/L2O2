using System;
using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class EnumerableBase<T> : IConsumableSeq<T>
    {
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public abstract IEnumerator<T> GetEnumerator();
        public abstract IConsumableSeq<U> Transform<U>(ISeqTransform<T, U> transform);
        public abstract Result Consume<Result>(Func<SeqConsumer<T, Result>> getConsumer);
    }
}
