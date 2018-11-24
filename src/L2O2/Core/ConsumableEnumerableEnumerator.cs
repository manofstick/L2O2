using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ConsumableEnumerableEnumerator<T, TResult> : ConsumableEnumerator<TResult>
    {
        private IEnumerable<T> enumerable;
        private IEnumerator<T> enumerator;
        private ConsumerActivity<T, TResult> activity = null;

        internal override Chain<TResult> StartOfChain => activity;

        private ConsumableEnumerableEnumerator(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
            activity = null;
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

        tryAgain:
            if (!enumerator.MoveNext() || Halted)
            {
                activity.ChainComplete(ref result);
                return false;
            }

            if (!activity.ProcessNext(enumerator.Current, ref result))
                goto tryAgain;

            return true;
        }
    }
}
