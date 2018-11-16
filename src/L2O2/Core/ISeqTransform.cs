namespace L2O2.Core
{
    internal interface ISeqTransform<T,U>
    {
        bool TryAggregate<V>(ISeqTransform<U, V> next, out ISeqTransform<T, V> composite);
        SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<U,V> activity);
    }
}
