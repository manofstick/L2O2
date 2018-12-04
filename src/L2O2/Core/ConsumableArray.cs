using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class ConsumableArray<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly T[] array;

        public ConsumableArray(T[] array, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.array = array;

        public override Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second) =>
            new ConsumableArray<T, VV, W>(array, first, second);

        public override IEnumerator<V> GetEnumerator()
        {
            if (array.Length == 0)
                return Utils.EmptyEnumerator<V>.Instance;

            if (ReferenceEquals(first, IdentityTransform<T>.Instance))
            {
                if (second is SelectImpl<T, V> t2v)
                    return GetEnumerator_Select(t2v);
            }

            return ConsumableArrayEnumerator<T, V>.Create(array, this);
        }

        private IEnumerator<V> GetEnumerator_Select(SelectImpl<T, V> t2v)
        {
            var f = t2v.Selector;
            foreach (var item in array)
                yield return f(item);
        }

        public override Result Consume<Result>(Consumer<V, Result> consumer)
        {
            const int MaxLengthToAvoidPipelineCreationCost = 5;

            if (array.Length == 0)
                return Consume_Empty(consumer);
            else
            {
                var transform = GetTransform();

                if (array.Length <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
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
                for (var i = 0; i < array.Length; ++i)
                {
                    var processNextResult = transform.OwnedProcessNext(array[i], out var u);
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

        private TResult Consume_Pipeline<TResult>(ITransmutation<T, V> transform, Consumer<V, TResult> consumer)
        {
            var activity = transform.Compose(consumer);
            try
            {
                for (var i = 0; i < array.Length; ++i)
                {
                    var processNextResult = activity.ProcessNext(array[i]);
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

    }
}
