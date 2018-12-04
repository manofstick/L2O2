using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableList<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly List<T> list;

        public ConsumableList(List<T> list, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.list = list;

        public override Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second) =>
            new ConsumableList<T, VV, W>(list, first, second);

        public override IEnumerator<V> GetEnumerator() =>
            Impl.GetEnumerator(list, this);

        public override Result Consume<Result>(Consumer<V, Result> consumer) =>
            Impl.Consume(list, this, consumer);
    }
}
