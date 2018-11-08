namespace L2O2.Core
{
    abstract class SeqConsumerActivity
    {
        public abstract void ChainComplete(PipeIdx pipeIdx);
        public abstract void ChainDispose();
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
