using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class Consumable<T>
        : IEnumerable<T> 
    {
        public abstract object Tail { get; }
        public abstract Consumable<V> ReplaceTail<U, V>(ITransmutation<U, V> selectImpl);
        public abstract Consumable<U> AddTail<U>(ITransmutation<T, U> transform);

        public abstract List<T> ToList();

        public abstract Result Consume<Result>(Consumer<T, Result> consumer);
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
