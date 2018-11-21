namespace L2O2.Core
{
    abstract class Chain
    {
        public virtual void ChainComplete() { }
        public virtual void ChainDispose() { }
    }

    abstract class ConsumerActivity<T> : Chain
    {
        public abstract bool ProcessNext(T input);
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
