using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableListEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private List<T> list;
        List<T>.Enumerator enumerator;

        private ConsumerActivity<T, TResult> activity = null;

        protected override Chain<TResult> StartOfChain => activity;

        private ConsumableListEnumerator(List<T> list)
        {
            this.list = list;
            activity = null;
        }

        public override void ChainDispose()
        {
            if (list == null)
                enumerator.Dispose();
            list = null;
        }

        internal static IEnumerator<TResult> Create(List<T> list, ITransmutation<T, TResult> factory)
        {
            var listEnumerator = new ConsumableListEnumerator<T, TResult>(list);
            listEnumerator.activity = factory.Compose(listEnumerator);
            return listEnumerator;
        }

        public override bool MoveNext()
        {
            if (list != null)
            {
                enumerator = list.GetEnumerator();
                list = null;
            }

        tryAgain:
            if (!enumerator.MoveNext() || result.Halted)
            {
                activity.ChainComplete(ref result.Value);
                return false;
            }

            if (!activity.ProcessNext(enumerator.Current, ref result))
                goto tryAgain;

            return true;
        }
    }
}
