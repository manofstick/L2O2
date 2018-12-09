using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableEnumerableEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private IEnumerable<T> enumerable;
        private IEnumerator<T> enumerator;
        private Chain<T, ChainEnd> activity = null;

        internal override Chain StartOfChain => activity;

        public ConsumableEnumerableEnumerator(IEnumerable<T> enumerable, ITransmutation<T, TResult> factory)
        {
            this.enumerable = enumerable;
            activity = factory.Compose(this); ;
        }

        public override void ChainDispose()
        {
            if (enumerator != null)
            {
                enumerator.Dispose();
                enumerator = null;
            }
            enumerable = null;
            activity = null;
        }

        public override bool MoveNext()
        {
            if (enumerable != null)
            {
                enumerator = enumerable.GetEnumerator();
                enumerable = null;
            }

        tryAgain:
            if (!enumerator.MoveNext() || state.IsStopped())
            {
                activity.ChainComplete();
                return false;
            }

            state = activity.ProcessNext(enumerator.Current);
            if (!state.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
