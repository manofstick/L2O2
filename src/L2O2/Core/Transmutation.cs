using System;

namespace L2O2.Core
{
    abstract class Transmutation<T, U> : ITransmutation<T, U>
    {
        public abstract Chain<T, V> Compose<V>(Chain<U, V> activity);

        public virtual ProcessNextResult ProcessNextStateless(T t, out U u)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsStateless() => false;
    }
}
