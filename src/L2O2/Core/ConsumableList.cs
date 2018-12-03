using System;
using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class ConsumableList<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly List<T> list;

        public ConsumableList(List<T> list, ITransmutation<T, U> first, ITransmutation<U, V> second)
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

            return ConsumableListEnumerator<T, V>.Create(list, this);
        }

        private IEnumerator<V> GetEnumerator_Select(SelectImpl<T, V> t2v)
        {
            var f = t2v.Selector;
            foreach (var item in list)
                yield return f(item);
        }

        public override Consumable<W> AddTail<W>(ITransmutation<V, W> next)
        {
            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
                return new ConsumableList<T, V, W>(list, (ITransmutation<T, V>)second, next);

            return new ConsumableList<T, V, W>(list, new CompositionTransform<T, U, V>(first, second), next);
        }

        public override Result Consume<Result>(Consumer<V, Result> consumer)
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

        private static Result Consume_Empty<Result>(Consumer<V, Result> consumer)
        {
            try { consumer.ChainComplete(); }
            finally { consumer.ChainDispose(); }
            return consumer.Result;
        }

        private TResult Consume_Owned<TResult>(ITransmutation<T, V> transform, Consumer<V, TResult> consumer)
        {
            try
            {
                // don't use index of list to ensure list modifications throw exceptions
                foreach(var t in list)
                {
                    var processNextResult = transform.OwnedProcessNext(t, out var u);
                    if (processNextResult.IsOK())
                        processNextResult = consumer.ProcessNext(u);
                    if (processNextResult.IsHalted())
                        break;
                }
                consumer.ChainComplete();
            }
            finally
            {
                consumer.ChainDispose();
            }
            return consumer.Result;
        }

        private TResult Consume_Pipeline<TResult>(ITransmutation<T, V>  transform, Consumer<V, TResult> consumer)
        {
            var activity = transform.Compose(consumer);
            try
            {
                // don't use index of list to ensure list modifications throw exceptions
                foreach (var t in list)
                {
                    var processNextResult = activity.ProcessNext(t);
                    if (processNextResult.IsHalted())
                        break;
                }
                activity.ChainComplete();
            }
            finally
            {
                activity.ChainDispose();
            }
            return consumer.Result;
        }

        public override Consumable<W> ReplaceTail<U_alias, W>(ITransmutation<U_alias, W> selectImpl)
        {
            return new ConsumableList<T, U, W>(list, first, (ITransmutation<U, W>)selectImpl);
        }
    }
}
