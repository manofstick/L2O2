using System;

namespace L2O2.Core
{
    internal interface IConsumableWithCompositionConstruct<T>
    {
        Consumable<V> Create<U, V>(ITransmutation<T, U> first, ITransmutation<U, V> second);
    }

    internal abstract class ConsumableWithComposition<T, U, V> : Consumable<V>, ITransmutation<T, V>, IConsumableWithCompositionConstruct<T>
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
            if (processNextResult.IsOK())
                return second.OwnedProcessNext(u, out v);

            v = default(V);
            return processNextResult;
        }

        Chain<T, W> ITransmutation<T, V>.Compose<W>(Chain<V, W> next)
        {
            return first.Compose(second.Compose(next));
        }

        public override Consumable<W> AddTail<W>(ITransmutation<V, W> next)
        {
            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
                return Create((ITransmutation<T, V>)second, next);

            return Create(CompositionTransform.Combine(first, second), next);
        }

        public override Consumable<W> ReplaceTail<U_alias, W>(ITransmutation<U_alias, W> selectImpl)
        {
            System.Diagnostics.Debug.Assert(typeof(U) == typeof(U_alias));
            return Create(first, (ITransmutation<U, W>)selectImpl);
        }

        public abstract Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second);
    }
}