using System.Collections.Generic;

namespace L2O2.Core
{
    internal class EnumerableEnumerator<T, TResult> : EnumeratorBase<TResult>
    {
        private IEnumerable<T> enumerable;
        private IEnumerator<T> enumerator;
        private SeqConsumerActivity<T, TResult> activity = null;

        internal override SeqConsumerActivity Activity => activity;

        private EnumerableEnumerator(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
            activity = null;
        }

        internal static IEnumerator<TResult> Create(IEnumerable<T> enumerable, ISeqTransform<T, TResult> factory)
        {
            var arrayEnumerator = new EnumerableEnumerator<T, TResult>(enumerable);
            arrayEnumerator.activity = factory.Compose(arrayEnumerator, arrayEnumerator);
            return arrayEnumerator;
        }

        public override bool MoveNext()
        {
            if (SeqState == SeqProcessNextStates.NotStarted)
            {
                enumerator = enumerable.GetEnumerator();
                enumerable = null;
            }

            SeqState = SeqProcessNextStates.InProcess;
            while (enumerator.MoveNext())
            {
                if (Halted)
                    break;

                if (activity.ProcessNext(enumerator.Current))
                    return true;
            }
            SeqState = SeqProcessNextStates.Finished;
            activity.ChainComplete();
            return false;
        }
    }
}
