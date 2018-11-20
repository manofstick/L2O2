using System;

namespace L2O2.Core
{
    internal abstract class EnumerableWithComposition<T, U, V> : Consumable<V>, ISeqTransform<T, V>
    {
        protected readonly ISeqTransform<T, U> first;
        protected readonly ISeqTransform<U, V> second;

        public override ISeqTransform<V> Tail => second;

        protected EnumerableWithComposition(ISeqTransform<T, U> first, ISeqTransform<U, V> second)
        {
            this.first = first;
            this.second = second;
        }

        protected ISeqTransform<T, V> GetTransform()
        {
            return ReferenceEquals(first, IdentityTransform<T>.Instance)
                    ? (ISeqTransform<T, V>)second
                    : this;
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