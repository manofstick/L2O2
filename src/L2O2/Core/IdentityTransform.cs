namespace L2O2.Core
{
    class IdentityTransform<T> : ISeqTransform<T,T>
    {
        public static ISeqTransform<T, T> Instance { get; } = new IdentityTransform<T>();

        public SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<T, V> next)
        {
            return next;
        }

        public bool OwnedProcessNext(T t, out T u)
        {
            u = t;
            return true;
        }

        public bool TryAggregate<V>(ISeqTransform<T, V> next, out ISeqTransform<T, V> composite)
        {
            composite = next;
            return true;
        }

        public bool TryOwn()
        {
            return true;
        }
    }
}
