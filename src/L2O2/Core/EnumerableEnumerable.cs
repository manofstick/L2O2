using System;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal class EnumerableEnumerable<T, U> : EnumerableWithTransform<T, U>
    {
        private readonly IEnumerable<T> enumerable;

        public EnumerableEnumerable(IEnumerable<T> enumerable, ISeqTransform<T, U> transform)
            : base(transform)
        {
            this.enumerable = enumerable;
        }

        public override IEnumerator<U> GetEnumerator()
        {
            return EnumerableEnumerator<T, U>.Create(enumerable, this);
        }

        public override IConsumableSeq<V> Transform<V>(ISeqTransform<U, V> next)
        {
            return new EnumerableEnumerable<T, V>(enumerable, CompositionTransform<T, U, V>.Combine(transform, next));
        }

        public override TResult Consume<TResult>(Func<SeqConsumer<U, TResult>> getConsumer)
        {
            var consumer = CreatePipeline(getConsumer, out var activity);
            try
            {
                foreach(var item in enumerable)
                { 
                    if (consumer.Halted)
                        break;

                    activity.ProcessNext(item);
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
