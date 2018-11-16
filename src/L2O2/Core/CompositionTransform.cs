using System;
using System.Collections.Generic;
using System.Text;

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
            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
                return (ISeqTransform<T, V>)second;

		    return new CompositionTransform<T, U, V>(first, second);
	    }

	    public SeqConsumerActivity<T, W> Compose<W>(ISeqConsumer outOfBand, SeqConsumerActivity<V, W> next)
	    {
		    return first.Compose(outOfBand, second.Compose(outOfBand, next));
	    }
    }
}
