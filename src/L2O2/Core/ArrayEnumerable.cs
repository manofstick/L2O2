﻿using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class ArrayEnumerable<T, U, V> : EnumerableWithComposition<T, U, V>
    {
        private readonly T[] array;

        public ArrayEnumerable(T[] array, ISeqTransform<T, U> first, ISeqTransform<U, V> second)
            : base(first, second)
        {
            this.array = array;
        }

        public override IEnumerator<V> GetEnumerator()
        {
            if (array.Length == 0)
                return Utils.EmptyEnumerator<V>.Instance;

            if (ReferenceEquals(first, IdentityTransform<T>.Instance) && second is SelectImpl<T, V> t2v)
                return GetEnumerator_Select(t2v);

            return ArrayEnumerator<T, V>.Create(array, this);
        }

        private IEnumerator<V> GetEnumerator_Select(SelectImpl<T, V> t2v)
        {
            var f = t2v.selector;
            foreach (var item in array)
                yield return f(item);
        }

        public override Consumable<W> Transform<W>(ISeqTransform<V, W> next)
        {
            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
                return new ArrayEnumerable<T, V, W>(array, (ISeqTransform<T, V>)second, next);

            return new ArrayEnumerable<T, V, W>(array, new CompositionTransform<T, U, V>(first, second), next);
        }

        public override Result Consume<Result>(SeqConsumer<V, Result> consumer)
        {
            const int MaxLengthToAvoidPipelineCreationCost = 5;

            if (array.Length == 0)
                return consumer.Result;
            else
            {
                var transform = GetTransform();

                if (array.Length <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
                    return Consume_Owned(transform, consumer);

                return Consume_Pipeline(transform, consumer);
            }
        }

        private TResult Consume_Owned<TResult>(ISeqTransform<T, V> transform, SeqConsumer<V, TResult> consumer)
        {
            try
            {
                for (var i = 0; i < array.Length; ++i)
                {
                    if (consumer.Halted)
                        break;

                    if (transform.OwnedProcessNext(array[i], out var u))
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

        private TResult Consume_Pipeline<TResult>(ISeqTransform<T, V> transform, SeqConsumer<V, TResult> consumer)
        {
            var activity = transform.Compose(consumer, consumer);
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

        public override Consumable<W> ReplaceTail<U_alias, W>(ISeqTransform<U_alias, W> selectImpl)
        {
            return new ArrayEnumerable<T, U, W>(array, first, (ISeqTransform<U,W>)selectImpl);
        }
    }
}
