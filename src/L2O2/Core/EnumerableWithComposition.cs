using System;

namespace L2O2.Core
{
    internal abstract class EnumerableWithComposition<T, U, V> : EnumerableBase<V>, ISeqTransform<T, V>
    {
        protected readonly ISeqTransform<T, U> first;
        protected readonly ISeqTransform<U, V> second;

        protected EnumerableWithComposition(ISeqTransform<T, U> first, ISeqTransform<U, V> second)
        {
            this.first = first;
            this.second = second;
        }

        bool ISeqTransform<T, V>.TryAggregate<W>(ISeqTransform<V, W> next, out ISeqTransform<T, W> composite)
        {
            if (second.TryAggregate(next, out var secondAndNext))
            {
                composite = new CompositionTransform<T, U, W>(first, secondAndNext);
                return true;
            }

            composite = null;
            return false;
        }

        bool ISeqTransform<T, V>.TryOwn()
        {
            return first.TryOwn() && second.TryOwn();
        }

        bool ISeqTransform<T, V>.OwnedProcessNext(T t, out V v)
        {
            if (first.OwnedProcessNext(t, out var u))
                return second.OwnedProcessNext(u, out v);

            v = default(V);
            return false;
        }

        SeqConsumerActivity<T, W> ISeqTransform<T, V>.Compose<W>(ISeqConsumer outOfBand, SeqConsumerActivity<V, W> next)
        {
            return first.Compose(outOfBand, second.Compose(outOfBand, next));
        }
    }
}