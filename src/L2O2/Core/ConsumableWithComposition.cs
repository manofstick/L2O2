using System;

namespace L2O2.Core
{
    internal abstract class ConsumableWithComposition<T, U, V> : Consumable<V>, ITransmutation<T, V>
    {
        protected readonly ITransmutation<T, U> first;
        protected readonly ITransmutation<U, V> second;

        public override ITransmutation<V> Tail => second;

        protected ConsumableWithComposition(ITransmutation<T, U> first, ITransmutation<U, V> second)
        {
            this.first = first;
            this.second = second;
        }

        protected ITransmutation<T, V> GetTransform()
        {
            return ReferenceEquals(first, IdentityTransform<T>.Instance)
                    ? (ITransmutation<T, V>)second
                    : this;
        }

        bool ITransmutation<T, V>.TryOwn()
        {
            return first.TryOwn() && second.TryOwn();
        }

        ProcessNextResult ITransmutation<T, V>.OwnedProcessNext(T t, out V v)
        {
            var processNextResult = first.OwnedProcessNext(t, out var u);
            if (processNextResult == ProcessNextResult.OK)
                return second.OwnedProcessNext(u, out v);

            v = default(V);
            return processNextResult;
        }

        ConsumerActivity<T, W> ITransmutation<T, V>.Compose<W>(ConsumerActivity<V, W> next)
        {
            return first.Compose(second.Compose(next));
        }
    }
}