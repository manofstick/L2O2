using System;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal class ListEnumerable<T, U> : EnumerableWithTransform<T, U>
    {
        private readonly List<T> list;

        public ListEnumerable(List<T> list, ISeqTransform<T, U> transform)
            : base(transform)
        {
            this.list = list;
        }

        public override IEnumerator<U> GetEnumerator()
        {
            return ListEnumerator<T, U>.Create(list, this);
        }

        public override IConsumableSeq<V> Transform<V>(ISeqTransform<U, V> next)
        {
            return new ListEnumerable<T, V>(list, new CompositionTransform<T, U, V>(transform, next));
        }

        public override TResult Consume<TResult>(Func<SeqConsumer<U, TResult>> getConsumer)
        {
            var consumer = CreatePipeline(getConsumer, out var activity);
            try
            {
                for (var i = 0; i < list.Count; ++i)
                {
                    if (consumer.Halted)
                        break;

                    activity.ProcessNext(list[i]);
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
