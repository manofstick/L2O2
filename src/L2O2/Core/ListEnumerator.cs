using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ListEnumerator<T, TResult> : EnumeratorBase<TResult>
    {
        private List<T> list;
        private int idx;
        private SeqConsumerActivity<T, TResult> activity = null;

        internal override SeqConsumerActivity Activity => activity;

        private ListEnumerator(List<T> list)
        {
            this.list = list;
            activity = null;
        }

        internal static IEnumerator<TResult> Create(List<T> list, ISeqTransform<T, TResult> factory)
        {
            var listEnumerator = new ListEnumerator<T, TResult>(list);
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
