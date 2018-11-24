namespace L2O2.Core
{
    internal interface ITransmutation<T> { }

    internal interface ITransmutation<T,U> : ITransmutation<U>
    {
        bool TryOwn();
        bool OwnedProcessNext(T t, out U u);

        ConsumerActivity<T, V, Result> Compose<V, Result>(IOutOfBand consumer, ConsumerActivity<U,V, Result> activity);
    }
}
