using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableArrayEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private T[] array;
        private int idx;
        private Chain<T, ChainEnd> activity = null;

        internal override Chain StartOfChain => activity;

        public ConsumableArrayEnumerator(T[] array, ITransmutation<T, TResult> factory)
        {
            this.array = array;
            activity = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            array = null;
            activity = null;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (idx >= array.Length || state.IsStopped())
            {
                activity.ChainComplete();
                return false;
            }

            state = activity.ProcessNext(array[idx++]);
            if (!state.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
