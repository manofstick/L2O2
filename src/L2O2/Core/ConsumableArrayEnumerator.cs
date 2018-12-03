using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableArrayEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private T[] array;
        private int idx;
        private ConsumerActivity<T, TResult> activity = null;

        internal override Chain StartOfChain => activity;

        private ConsumableArrayEnumerator(T[] array)
        {
            this.array = array;
            activity = null;
        }

        public override void ChainDispose()
        {
            array = null;
            activity = null;
        }

        internal static IEnumerator<TResult> Create(T[] array, ITransmutation<T, TResult> factory)
        {
            var arrayEnumerator = new ConsumableArrayEnumerator<T, TResult>(array);
            arrayEnumerator.activity = factory.Compose(arrayEnumerator);
            return arrayEnumerator;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (idx >= array.Length || processNextResult.IsHalted())
            {
                activity.ChainComplete();
                return false;
            }

            processNextResult = activity.ProcessNext(array[idx++]);
            if (!processNextResult.IsOK())
                goto tryAgain;

            return true;
        }
    }
}
