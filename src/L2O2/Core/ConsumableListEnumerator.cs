using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableListEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private List<T> list;
        List<T>.Enumerator enumerator;

        private Chain<T, ChainEnd> activity = null;

        internal override Chain StartOfChain => activity;

        public ConsumableListEnumerator(List<T> list, ITransmutation<T, TResult> factory)
        {
            this.list = list;
            activity = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            if (list == null)
                enumerator.Dispose();
            list = null;
        }

        public override bool MoveNext()
        {
            if (list != null)
            {
                enumerator = list.GetEnumerator();
                list = null;
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
