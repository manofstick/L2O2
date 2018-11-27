namespace L2O2.Core
{
    abstract class Chain<Result>
    {
        public abstract void ChainComplete(ref Result result);
        public abstract void ChainDispose();
    }

    struct Status<T>
    {
        public bool Halted;
        public T Value;
    }

    abstract class ConsumerActivity<T, Result> : Chain<Result>
    {
        public abstract bool ProcessNext(T input, ref Status<Result> result);
    }

    abstract class ConsumerActivity<T, U, Result> : ConsumerActivity<T, Result>
    {
    }

    abstract class ConsumerActivity<T, U, V, Result> : ConsumerActivity<T,V, Result>
    {
        protected readonly ConsumerActivity<U, V, Result> next;

        protected ConsumerActivity(ConsumerActivity<U, V, Result> next)
        {
            this.next = next;
        }

        public override void ChainComplete(ref Result result) => next.ChainComplete(ref result);
        public override void ChainDispose() => next.ChainDispose();
    }
}
