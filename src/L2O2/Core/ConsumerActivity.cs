using System;

namespace L2O2.Core
{
    abstract class Chain
    {
        public abstract void ChainComplete();
        public abstract void ChainDispose();
    }

    [Flags]
    enum ProcessNextResult
    {
        OK             = 0x00,
        Filtered       = 0x02,
        Halted         = 0x80,
        HaltedActivity = 0x81,
        HaltedConsumer = 0x82,
    }

    abstract class ConsumerActivity<T> : Chain
    {
        public abstract ProcessNextResult ProcessNext(T input);
    }

    abstract class ConsumerActivity<T, U> : ConsumerActivity<T>
    {
    }

    abstract class ConsumerActivity<T, U, V> : ConsumerActivity<T,V>
    {
        protected readonly ConsumerActivity<U, V> next;

        protected ConsumerActivity(ConsumerActivity<U, V> next)
        {
            this.next = next;
        }

        public override void ChainComplete() => next.ChainComplete();
        public override void ChainDispose() => next.ChainDispose();
    }
}
