using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableArrayEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private T[] array;
        private int idx;
        private ConsumerActivity<T, TResult> activity = null;

        internal override ConsumerActivity Activity => activity;

        private ConsumableArrayEnumerator(T[] array)
        {
            this.array = array;
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
            while (idx < array.Length && !Halted)
            {
                if (activity.ProcessNext(array[idx++]))
                    return true;
            }
            activity.ChainComplete();
            return false;
        }
    }
}
