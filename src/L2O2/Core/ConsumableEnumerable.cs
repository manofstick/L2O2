using System;
using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class ConsumableEnumerable<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly IEnumerable<T> enumerable;

        public ConsumableEnumerable(IEnumerable<T> enumerable, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.enumerable = enumerable;

        public override Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second) =>
            new ConsumableEnumerable<T, VV, W>(enumerable, first, second);

        public override IEnumerator<V> GetEnumerator()
        {
            if (ReferenceEquals(first, IdentityTransform<T>.Instance) && second is SelectImpl<T, V> t2v)
                return GetEnumerator_Select(t2v);

            return ConsumableEnumerableEnumerator<T, V>.Create(enumerable, this);
        }

        private IEnumerator<V> GetEnumerator_Select(SelectImpl<T, V> t2u)
        {
            var f = t2u.Selector;
            foreach (var item in enumerable)
                yield return f(item);
        }

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer)
        {
            var transform = (ITransmutation<T, V>)this;
            var activity = transform.Compose(consumer);
            try
            {
                foreach(var item in enumerable)
                { 
                    var processNextResult = activity.ProcessNext(item);
                    if (processNextResult.IsHalted())
                        break;
                }
                activity.ChainComplete();
            }
            finally
            {
                activity.ChainDispose();
            }
            return consumer.Result;
        }
    }
}
