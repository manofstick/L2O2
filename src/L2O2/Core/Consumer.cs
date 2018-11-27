using System;

namespace L2O2.Core
{
    abstract class Consumer<T, Result>
        : ConsumerActivity<T, T, Result>
    {
        protected Consumer(Result initalResult)
        {
            InitialResult = initalResult;
        }

        public Result InitialResult { get; }

        public override void ChainComplete(ref Result result) { }
        public override void ChainDispose() { }
    }
}
