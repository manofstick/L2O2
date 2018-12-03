namespace L2O2.Core
{
    abstract class Consumer<T, R>
        : ConsumerActivity<T, T>
    {
        protected Consumer(R initalResult)
        {
            Result = initalResult;
        }

        public R Result { get; protected set; }

        public override void ChainComplete() { }
        public override void ChainDispose() { }
    }
}
