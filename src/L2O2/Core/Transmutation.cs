using System;

namespace L2O2.Core
{
    abstract class Transmutation<T, U> : ITransmutation<T, U>
    {
        public abstract ConsumerActivity<T, V> Compose<V>(ConsumerActivity<U, V> activity);

        public virtual bool OwnedProcessNext(T t, out U u)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryOwn() => false;
    }
}
