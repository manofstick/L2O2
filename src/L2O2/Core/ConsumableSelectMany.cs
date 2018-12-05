using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableSelectMany<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly Consumable<IEnumerable<T>> consumableEnumerable;

        public ConsumableSelectMany(Consumable<IEnumerable<T>> enumerable, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.consumableEnumerable = enumerable;

        public override Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second) =>
            new ConsumableSelectMany<T, VV, W>(consumableEnumerable, first, second);

        public override IEnumerator<V> GetEnumerator() =>
            Impl.GetEnumerator(consumableEnumerable, this);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            Impl.Consume(consumableEnumerable, this, consumer);
    }
}
