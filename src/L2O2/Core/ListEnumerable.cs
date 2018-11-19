using System;
using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class ListEnumerable<T, U, V> : EnumerableWithComposition<T, U, V>
    {
        private readonly List<T> list;

        public ListEnumerable(List<T> list, ISeqTransform<T, U> first, ISeqTransform<U, V> second)
            : base(first, second)
        {
            this.list = list;
        }

        public override IEnumerator<V> GetEnumerator()
        {
            if (list.Count == 0)
                return Utils.EmptyEnumerator<V>.Instance;

            if (ReferenceEquals(first, IdentityTransform<T>.Instance) && second is SelectImpl<T, V> t2v)
                return GetEnumerator_Select(t2v);

            return ListEnumerator<T, V>.Create(list, this);
        }

        private IEnumerator<V> GetEnumerator_Select(SelectImpl<T, V> t2v)
        {
            var f = t2v.selector;
            foreach (var item in list)
                yield return f(item);
        }

        public override IConsumableSeq<W> Transform<W>(ISeqTransform<V, W> next)
        {
            if (second.TryAggregate(next, out var composite))
                return new ListEnumerable<T, U, W>(list, first, composite);

            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
                return new ListEnumerable<T, V, W>(list, (ISeqTransform<T, V>)second, next);

            return new ListEnumerable<T, V, W>(list, new CompositionTransform<T, U, V>(first, second), next);
        }

        public override Result Consume<Result>(SeqConsumer<V, Result> consumer)
        {
            const int MaxLengthToAvoidPipelineCreationCost = 5;

            var transform = (ISeqTransform<T, V>)this;
            if (list.Count == 0)
                return consumer.Result;
            else if (list.Count <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
                return Consume_Owned(consumer);

            return Consume_Pipeline(consumer);
        }

        private TResult Consume_Owned<TResult>(SeqConsumer<V, TResult> consumer)
        {
            try
            {
                var transform = (ISeqTransform<T, V>)this;
                for (var i = 0; i < list.Count; ++i)
                {
                    if (consumer.Halted)
                        break;

                    if (transform.OwnedProcessNext(list[i], out var u))
                        consumer.ProcessNext(u);
                }
                consumer.ChainComplete();
            }
            finally
            {
                consumer.ChainDispose();
            }
            return consumer.Result;
        }

        private TResult Consume_Pipeline<TResult>(SeqConsumer<V, TResult> consumer)
        {
            var transform = (ISeqTransform<T, V>)this;
            var activity = transform.Compose(consumer, consumer);
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
