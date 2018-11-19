using System;
using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class EnumerableEnumerable<T, U, V> : EnumerableWithComposition<T, U, V>
    {
        private readonly IEnumerable<T> enumerable;

        public EnumerableEnumerable(IEnumerable<T> enumerable, ISeqTransform<T, U> first, ISeqTransform<U, V> second)
            : base(first, second)
        {
            this.enumerable = enumerable;
        }

        public override IEnumerator<V> GetEnumerator()
        {
            if (ReferenceEquals(first, IdentityTransform<T>.Instance) && second is SelectImpl<T, V> t2v)
                return GetEnumerator_Select(t2v);

            return EnumerableEnumerator<T, V>.Create(enumerable, this);
        }

        private IEnumerator<V> GetEnumerator_Select(SelectImpl<T, V> t2u)
        {
            var f = t2u.selector;
            foreach (var item in enumerable)
                yield return f(item);
        }

        public override IConsumableSeq<W> Transform<W>(ISeqTransform<V, W> next)
        {
            if (second.TryAggregate(next, out var composite))
                return new EnumerableEnumerable<T, U, W>(enumerable, first, composite);

            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
                return new EnumerableEnumerable<T, V, W>(enumerable, (ISeqTransform<T, V>)second, next);

            return new EnumerableEnumerable<T, V, W>(enumerable, new CompositionTransform<T, U, V>(first, second), next);
        }

        public override TResult Consume<TResult>(SeqConsumer<V, TResult> consumer)
        {
            var transform = (ISeqTransform<T, V>)this;
            var activity = transform.Compose(consumer, consumer);
            try
            {
                foreach(var item in enumerable)
                { 
                    if (consumer.Halted)
                        break;

                    activity.ProcessNext(item);
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
