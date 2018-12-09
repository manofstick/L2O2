namespace L2O2.Core
{
    class IdentityTransform<T> : ITransmutation<T,T>
    {
        public static ITransmutation<T, T> Instance { get; } = new IdentityTransform<T>();

        public Chain<T, V> Compose<V>(Chain<T, V> next)
        {
            return next;
        }

        public ProcessNextResult ProcessNextStateless(T t, out T u)
        {
            u = t;
            return ProcessNextResult.Flow;
        }

        public bool IsStateless()
        {
            return true;
        }
    }
}
