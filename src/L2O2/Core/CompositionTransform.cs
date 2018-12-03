using System;
using System.Collections.Generic;
using System.Text;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class CompositionTransform<T, U, V> : ITransmutation<T, V>
    {
	    internal ITransmutation<U, V> second;
	    internal ITransmutation<T, U> first;

	    public CompositionTransform(ITransmutation<T, U> first, ITransmutation<U, V> second)
	    {
		    this.first = first;
		    this.second = second;
	    }

	    internal static ITransmutation<T, V> Combine(ITransmutation<T, U> first, ITransmutation<U, V> second)
	    {
            return new CompositionTransform<T, U, V>(first, second);
	    }

        public ConsumerActivity<T, W> Compose<W>(ConsumerActivity<V, W> next)
	    {
		    return first.Compose(second.Compose(next));
	    }

        public bool TryOwn()
        {
            return first.TryOwn() && second.TryOwn();
        }

        public ProcessNextResult OwnedProcessNext(T t, out V v)
        {
            var processNextResult = first.OwnedProcessNext(t, out var u);
            if (processNextResult == ProcessNextResult.OK)
                return second.OwnedProcessNext(u, out v);

            v = default(V);
            return processNextResult;
        }
    }
}
