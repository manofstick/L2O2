using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ListEnumerator<T, TResult> : EnumeratorBase<TResult>
    {
        private readonly List<T> list;
        private SeqConsumerActivity<T, TResult> activity;
        private int idx;

        internal override SeqConsumerActivity Activity => activity;

        private ListEnumerator(List<T> list)
        {
            this.list = list;
        }

        internal static IEnumerator<TResult> Create(List<T> list, EnumerableWithTransform<T, TResult> factory)
        {
            var enumerator = new ListEnumerator<T, TResult>(list);
            enumerator.activity = factory.CreateActivityPipeline(enumerator);
            return enumerator;
        }

        public override bool MoveNext()
        {
            SeqState = SeqProcessNextStates.InProcess;
            while (!Halted && idx < list.Count)
            {
                if (activity.ProcessNext(list[idx++]))
                    return true;
            }
            SeqState = SeqProcessNextStates.Finished;
            activity.ChainComplete();
            return false;
        }
    }
}
