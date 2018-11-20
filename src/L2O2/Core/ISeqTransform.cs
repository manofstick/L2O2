namespace L2O2.Core
{
    internal interface ISeqTransform<T> { }

    internal interface ISeqTransform<T,U> : ISeqTransform<U>
    {
        bool TryOwn();
        bool OwnedProcessNext(T t, out U u);

        SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<U,V> activity);
    }
}
