using System;

namespace L2O2.Core
{
    internal interface IConsumableWithCompositionConstruct<T>
    {
        Consumable<V> Create<U, V>(ITransmutation<T, U> first, ITransmutation<U, V> second);
    }

    internal interface IComposition<T,U,V>
    {
        ITransmutation<T, U> First { get; }
        ITransmutation<U, V> Second { get; }

        ITransmutation<T, V> Composed { get; }
    }

    internal abstract class ConsumableWithComposition<T, U, V> : Consumable<V>, ITransmutation<T, V>, IConsumableWithCompositionConstruct<T>, IComposition<T,U,V>
    {
        protected readonly ITransmutation<T, U> first;
        protected readonly ITransmutation<U, V> second;

        protected ConsumableWithComposition(ITransmutation<T, U> first, ITransmutation<U, V> second) =>
            (this.first, this.second) = (first, second);

        public ITransmutation<T, U> First => first;
        public ITransmutation<U, V> Second => second;
        public ITransmutation<T, V> Composed => GetTransform();

        protected ITransmutation<T, V> GetTransform()
        {
            return ReferenceEquals(first, IdentityTransform<T>.Instance)
                    ? (ITransmutation<T, V>)second
                    : this;
        }

        bool ITransmutation<T, V>.TryOwn() =>
            first.TryOwn() && second.TryOwn();

        ProcessNextResult ITransmutation<T, V>.OwnedProcessNext(T t, out V v)
        {
            var processNextResult = first.OwnedProcessNext(t, out var u);
            if (processNextResult.IsOK())
                return second.OwnedProcessNext(u, out v);

            v = default(V);
            return processNextResult;
        }

        Chain<T, W> ITransmutation<T, V>.Compose<W>(Chain<V, W> next) =>
            first.Compose(second.Compose(next));

        public override object Tail => second;

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