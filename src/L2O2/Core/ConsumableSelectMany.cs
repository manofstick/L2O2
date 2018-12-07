using System;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableSelectMany<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly Consumable<IEnumerable<T>> consumableEnumerable;

        public ConsumableSelectMany(Consumable<IEnumerable<T>> enumerable, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.consumableEnumerable = enumerable;

        public override Consumable<X> Create<W, X>(ITransmutation<T, W> first, ITransmutation<W, X> second) =>
            new ConsumableSelectMany<T, W, X>(consumableEnumerable, first, second);

        public override IEnumerator<V> GetEnumerator() =>
            Impl.GetEnumerator(consumableEnumerable, this);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            Impl.Consume(consumableEnumerable, this, consumer);
    }

    internal class ConsumableSelectMany<TSource, TCollection, T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly Consumable<(TSource, IEnumerable<TCollection>)> consumableEnumerable;
        private readonly Func<TSource, TCollection, T> resultSelector;

        public ConsumableSelectMany(Consumable<(TSource,IEnumerable<TCollection>)> enumerable, Func<TSource, TCollection, T> resultSelector, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            (this.consumableEnumerable, this.resultSelector) = (enumerable, resultSelector);

        public override Consumable<X> Create<W, X>(ITransmutation<T, W> first, ITransmutation<W, X> second) =>
            new ConsumableSelectMany<TSource, TCollection, T, W, X>(consumableEnumerable, resultSelector, first, second);

        public override IEnumerator<V> GetEnumerator() =>
            Impl.GetEnumerator(consumableEnumerable, resultSelector, this);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            Impl.Consume(consumableEnumerable, resultSelector, this, consumer);
    }
}
