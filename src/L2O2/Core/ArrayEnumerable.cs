using System;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ArrayEnumerable<T,U> : EnumerableWithTransform<T, U>
    {
	    private readonly T[] array;

	    public ArrayEnumerable(T[] array, ISeqTransform<T, U> transform)
            : base(transform)
	    {
		    this.array = array;
	    }

	    public override IEnumerator<U> GetEnumerator()
	    {
		    return ArrayEnumerator<T, U>.Create(array, this);
	    }

	    public override IConsumableSeq<V> Transform<V>(ISeqTransform<U, V> next)
	    {
		    return new ArrayEnumerable<T,V>(array, CompositionTransform<T, U, V>.Combine(transform, next));
	    }

        public override TResult Consume<TResult>(Func<SeqConsumer<U, TResult>> getConsumer)
        {
		    var consumer = CreatePipeline(getConsumer, out var activity);
		    try
		    {
                for (var i = 0; i < array.Length; ++i)
                {
                    if (consumer.Halted)
                        break;

                    activity.ProcessNext(array[i]);
                }
			    activity.ChainComplete();
		    }
		    finally
		    {
			    activity.ChainDispose();
		    }
		    return consumer.Result;
	    }
    }
}
