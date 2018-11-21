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
            arrayEnumerator.activity = factory.Compose(arrayEnumerator, arrayEnumerator);
            return arrayEnumerator;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (idx >= array.Length || Halted)
            {
                activity.ChainComplete();
                return false;
            }

            if (!activity.ProcessNext(array[idx++]))
                goto tryAgain;

            return true;
        }
    }
}
