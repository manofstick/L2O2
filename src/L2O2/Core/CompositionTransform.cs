using System;
using System.Collections.Generic;
using System.Text;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class CompositionTransform<T, U, V> : ISeqTransform<T, V>
    {
	    internal ISeqTransform<U, V> second;
	    internal ISeqTransform<T, U> first;

	    private CompositionTransform(ISeqTransform<T, U> first, ISeqTransform<U, V> second)
	    {
		    this.first = first;
		    this.second = second;
	    }

	    internal static ISeqTransform<T, V> Combine(ISeqTransform<T, U> first, ISeqTransform<U, V> second)
	    {
            if (first.TryAggregate(second, out var composite))
                return composite;

		    return new CompositionTransform<T, U, V>(first, second);
	    }

        public bool TryAggregate<V1>(ISeqTransform<V, V1> next, out ISeqTransform<T, V1> composite)
        {
            composite = null;
            return false;
        }

        public SeqConsumerActivity<T, W> Compose<W>(ISeqConsumer outOfBand, SeqConsumerActivity<V, W> next)
	    {
		    return first.Compose(outOfBand, second.Compose(outOfBand, next));
	    }

        public bool TryOwn()
        {
            return first.TryOwn() && second.TryOwn();
        }

        public bool OwnedProcessNext(T t, out V v)
        {
            if (first.OwnedProcessNext(t, out var u))
                return second.OwnedProcessNext(u, out v);

            v = default(V);
            return false;
        }
    }
}
