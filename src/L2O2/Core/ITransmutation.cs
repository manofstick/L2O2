namespace L2O2.Core
{
    internal interface ITransmutation<T,U>
    {
        bool TryOwn();
        ProcessNextResult OwnedProcessNext(T t, out U u);

        Chain<T, V> Compose<V>(Chain<U,V> activity);
    }
}
