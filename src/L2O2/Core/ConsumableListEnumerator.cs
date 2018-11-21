using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableListEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private List<T> list;
        private int idx;
        private ConsumerActivity<T, TResult> activity = null;

        internal override ConsumerActivity Activity => activity;

        private ConsumableListEnumerator(List<T> list)
        {
            this.list = list;
            activity = null;
        }

        internal static IEnumerator<TResult> Create(List<T> list, ITransmutation<T, TResult> factory)
        {
            var listEnumerator = new ConsumableListEnumerator<T, TResult>(list);
            listEnumerator.activity = factory.Compose(listEnumerator, listEnumerator);
            return listEnumerator;
        }

        public override bool MoveNext()
        {
            //SeqState = SeqProcessNextStates.InProcess;
            while (idx < list.Count && !Halted)
            {
                if (activity.ProcessNext(list[idx++]))
                    return true;
            }
            //SeqState = SeqProcessNextStates.Finished;
            activity.ChainComplete();
            return false;
        }
    }
}
