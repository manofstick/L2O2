namespace L2O2.Core
{
    static class CompositionTransform
    {
        public static ITransmutation<T, V> Combine<T, U, V>(ITransmutation<T, U> first, ITransmutation<U, V> second) =>
            new CompositionTransform<T, U, V>(first, second);
    }

    internal class CompositionTransform<T, U, V> : ITransmutation<T, V>
    {
        private readonly ITransmutation<T, U> first;
        private readonly ITransmutation<U, V> second;

        public CompositionTransform(ITransmutation<T, U> first, ITransmutation<U, V> second) =>
            (this.first, this.second) = (first, second);

        public Chain<T, W> Compose<W>(Chain<V, W> next) =>
		    first.Compose(second.Compose(next));

        public bool TryOwn() =>
            first.TryOwn() && second.TryOwn();

        public ProcessNextResult OwnedProcessNext(T t, out V v)
        {
            var processNextResult = first.OwnedProcessNext(t, out var u);
            if (processNextResult.IsOK())
                return second.OwnedProcessNext(u, out v);

            v = default(V);
            return processNextResult;
        }
    }
}
