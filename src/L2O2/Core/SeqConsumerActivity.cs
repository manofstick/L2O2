namespace L2O2.Core
{
    abstract class SeqConsumerActivity
    {
        public virtual void ChainComplete() { }
        public virtual void ChainDispose() { }
    }

    abstract class SeqConsumerActivity<T>
        : SeqConsumerActivity
    {
        public abstract bool ProcessNext(T input);
    }

    abstract class SeqConsumerActivity<T,U>
        : SeqConsumerActivity<T>
    {
    }
}
