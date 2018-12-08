using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableArray<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly T[] array;

        public ConsumableArray(T[] array, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.array = array;

        public override Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second) =>
            new ConsumableArray<T, VV, W>(array, first, second);

        public override IEnumerator<V> GetEnumerator() =>
            Impl.GetEnumerator(array, this);

        public override Result Consume<Result>(Consumer<V, Result> consumer) =>
            Impl.Consume(array, this, consumer);

        public override List<V> ToList() =>
            Impl.ToList(this, array, this);
    }
}
