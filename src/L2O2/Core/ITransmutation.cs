namespace L2O2.Core
{
    internal interface ITransmutation<T> { }

    internal interface ITransmutation<T,U> : ITransmutation<U>
    {
        bool TryOwn();
        bool OwnedProcessNext(T t, out U u);

        ConsumerActivity<T, V> Compose<V>(IOutOfBand consumer, ConsumerActivity<U,V> activity);
    }
}
