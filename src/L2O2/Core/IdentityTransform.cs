namespace L2O2.Core
{
    class IdentityTransform<T> : ITransmutation<T,T>
    {
        public static ITransmutation<T, T> Instance { get; } = new IdentityTransform<T>();

        public Chain<T, V> Compose<V>(Chain<T, V> next)
        {
            return next;
        }

        public ProcessNextResult OwnedProcessNext(T t, out T u)
        {
            u = t;
            return ProcessNextResult.OK;
        }

        public bool TryOwn()
        {
            return true;
        }
    }
}
