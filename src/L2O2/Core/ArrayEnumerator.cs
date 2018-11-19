using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ArrayEnumerator<T, TResult> : EnumeratorBase<TResult>
    {
        private T[] array;
        private int idx;
        private SeqConsumerActivity<T, TResult> activity = null;

        internal override SeqConsumerActivity Activity => activity;

        private ArrayEnumerator(T[] array)
        {
            this.array = array;
            activity = null;
        }

        internal static IEnumerator<TResult> Create(T[] array, ISeqTransform<T, TResult> factory)
        {
            var arrayEnumerator = new ArrayEnumerator<T, TResult>(array);
            arrayEnumerator.activity = factory.Compose(arrayEnumerator, arrayEnumerator);
            return arrayEnumerator;
        }

        public override bool MoveNext()
        {
            //SeqState = SeqProcessNextStates.InProcess;
            while (idx < array.Length && !Halted)
            {
                if (activity.ProcessNext(array[idx++]))
                    return true;
            }
            //SeqState = SeqProcessNextStates.Finished;
            activity.ChainComplete();
            return false;
        }
    }
}
