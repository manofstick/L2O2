using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableEnumerable<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly IEnumerable<T> enumerable;

        public ConsumableEnumerable(IEnumerable<T> enumerable, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.enumerable = enumerable;

        public override Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second) =>
            new ConsumableEnumerable<T, VV, W>(enumerable, first, second);

        public override IEnumerator<V> GetEnumerator() =>
            Impl.GetEnumerator(enumerable, this);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            Impl.Consume(enumerable, this, consumer);

        public override List<V> ToList() =>
            Impl.ToList(this, enumerable, this);
    }
}
