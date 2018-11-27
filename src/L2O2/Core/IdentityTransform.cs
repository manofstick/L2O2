namespace L2O2.Core
{
    class IdentityTransform<T> : ITransmutation<T,T>
    {
        public static ITransmutation<T, T> Instance { get; } = new IdentityTransform<T>();

        public ConsumerActivity<T, V, Result> Compose<V, Result>(ConsumerActivity<T, V, Result> next)
        {
            return next;
        }

        public bool OwnedProcessNext(T t, out T u)
        {
            u = t;
            return true;
        }

        public bool TryOwn()
        {
            return true;
        }
    }
}
