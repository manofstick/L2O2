using System;
using System.Collections.Generic;
using static L2O2.Consumable;

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
            if (list.Count == 0)
                return Utils.EmptyEnumerator<U>.Instance;

            if (transform is SelectImpl<T, U> t2u)
                return GetEnumerator_Select(t2u);

            return ListEnumerator<T, U>.Create(list, this);
        }

        private IEnumerator<U> GetEnumerator_Select(SelectImpl<T, U> t2u)
        {
            var f = t2u.selector;
            foreach (var item in list)
                yield return f(item);
        }

        public override IConsumableSeq<V> Transform<V>(ISeqTransform<U, V> next)
        {
            return new ListEnumerable<T, V>(list, CompositionTransform<T, U, V>.Combine(transform, next));
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
