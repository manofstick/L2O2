﻿using System;
using System.Collections.Generic;
using static L2O2.Consumable;

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
            if (array.Length == 0)
                return Utils.EmptyEnumerator<U>.Instance;

            if (transform is SelectImpl<T, U> t2u)
                return GetEnumerator_Select(t2u);

		    return ArrayEnumerator<T, U>.Create(array, this);
	    }

        private IEnumerator<U> GetEnumerator_Select(SelectImpl<T, U> t2u)
        {
            var f = t2u.selector;
            foreach (var item in array)
                yield return f(item);
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
