using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableEnumerableEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private IEnumerable<T> enumerable;
        private IEnumerator<T> enumerator;
        private ConsumerActivity<T, TResult> activity = null;

        internal override ConsumerActivity Activity => activity;

        private ConsumableEnumerableEnumerator(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
            activity = null;
        }

        internal static IEnumerator<TResult> Create(IEnumerable<T> enumerable, ITransmutation<T, TResult> factory)
        {
            var arrayEnumerator = new ConsumableEnumerableEnumerator<T, TResult>(enumerable);
            arrayEnumerator.activity = factory.Compose(arrayEnumerator, arrayEnumerator);
            return arrayEnumerator;
        }

        public override bool MoveNext()
        {
            if (enumerable != null)
            {
                enumerator = enumerable.GetEnumerator();
                enumerable = null;
            }

            while (enumerator.MoveNext())
            {
                if (Halted)
                    break;

                if (activity.ProcessNext(enumerator.Current))
                    return true;
            }

            activity.ChainComplete();
            return false;
        }
    }
}
