﻿using System;
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
            var f = t2v.Selector;
            foreach (var item in list)
                yield return f(item);
        }

        public override Consumable<W> AddTail<W>(ISeqTransform<V, W> next)
        {
            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
                return new ListEnumerable<T, V, W>(list, (ISeqTransform<T, V>)second, next);

            return new ListEnumerable<T, V, W>(list, new CompositionTransform<T, U, V>(first, second), next);
        }

        public override Result Consume<Result>(SeqConsumer<V, Result> consumer)
        {
            if (list.Count == 0)
                return Consume_Empty(consumer);
            else
            {
                const int MaxLengthToAvoidPipelineCreationCost = 5;

                var transform = GetTransform();

                if (list.Count <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
                    return Consume_Owned(transform, consumer);

                return Consume_Pipeline(transform, consumer);
            }
        }

        private static Result Consume_Empty<Result>(SeqConsumer<V, Result> consumer)
        {
            try { consumer.ChainComplete(); }
            finally { consumer.ChainDispose(); }
            return consumer.Result;
        }

        private TResult Consume_Owned<TResult>(ISeqTransform<T, V> transform, SeqConsumer<V, TResult> consumer)
        {
            try
            {
                // don't use index of list to ensure list modifications throw exceptions
                foreach(var t in list)
                {
                    if (consumer.Halted)
                        break;

                    if (transform.OwnedProcessNext(t, out var u))
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

        private TResult Consume_Pipeline<TResult>(ISeqTransform<T, V>  transform, SeqConsumer<V, TResult> consumer)
        {
            var activity = transform.Compose(consumer, consumer);
            try
            {
                // don't use index of list to ensure list modifications throw exceptions
                foreach (var t in list)
                {
                    if (consumer.Halted)
                        break;

                    activity.ProcessNext(t);
                }
                activity.ChainComplete();
            }
            finally
            {
                activity.ChainDispose();
            }
            return consumer.Result;
        }

        public override Consumable<W> ReplaceTail<U_alias, W>(ISeqTransform<U_alias, W> selectImpl)
        {
            return new ListEnumerable<T, U, W>(list, first, (ISeqTransform<U, W>)selectImpl);
        }
    }
}
