namespace L2O2.Core
{
    abstract class ConsumerActivity
    {
        public virtual void ChainComplete() { }
        public virtual void ChainDispose() { }
    }

    abstract class ConsumerActivity<T>
        : ConsumerActivity
    {
        public abstract bool ProcessNext(T input);
    }

    abstract class ConsumerActivity<T,U>
        : ConsumerActivity<T>
    {
    }
}
