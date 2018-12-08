namespace L2O2.Core
{
    internal interface ITransmutation<T,U>
    {
        bool IsStateless();
        ProcessNextResult ProcessNextStateless(T t, out U u);

        Chain<T, V> Compose<V>(Chain<U,V> activity);
    }
}
