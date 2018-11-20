using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class Consumable<T>
        : IEnumerable<T> 
    {
        public abstract ISeqTransform<T> Tail { get; }
        public abstract Consumable<V> ReplaceTail<U, V>(ISeqTransform<U, V> selectImpl);
        public abstract Consumable<U> AddTail<U>(ISeqTransform<T, U> transform);

        public abstract Result Consume<Result>(SeqConsumer<T, Result> consumer);
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
