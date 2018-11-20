using System;

namespace L2O2.Core
{
    abstract class SeqTransform<T, U> : ISeqTransform<T, U>
    {
        public abstract SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<U, V> activity);

        public virtual bool OwnedProcessNext(T t, out U u)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryOwn() => false;
    }
}
