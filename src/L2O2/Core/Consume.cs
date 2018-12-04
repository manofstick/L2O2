using System.Collections.Generic;

namespace L2O2.Core
{
    static partial class Impl
    {
        public static Result Consume<T, U, V, Result>(T[] array, IComposition<T, U, V> composition, Consumer<V, Result> consumer)
        {
            if (array.Length == 0)
                return Empty(consumer);
            else
            {
                var transform = composition.Composed;

                const int MaxLengthToAvoidPipelineCreationCost = 5;
                if (array.Length <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
                    return Owned(array, transform, consumer);

                return Pipeline(array, transform, consumer);
            }
        }

        public static Result Consume<T, U, V, Result>(List<T> lst, IComposition<T, U, V> composition, Consumer<V, Result> consumer)
        {
            if (lst.Count == 0)
                return Empty(consumer);
            else
            {
                var transform = composition.Composed;

                const int MaxLengthToAvoidPipelineCreationCost = 5;
                if (lst.Count <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
                    return Owned(lst, transform, consumer);

                return Pipeline(lst, transform, consumer);
            }
        }

        public static Result Consume<T, U, V, Result>(IEnumerable<T> e, IComposition<T, U, V> composition, Consumer<V, Result> consumer) =>
            Pipeline(e, composition.Composed, consumer);

        private static Result Empty<V, Result>(Consumer<V, Result> consumer)
        {
            try { consumer.ChainComplete(); }
            finally { consumer.ChainDispose(); }
            return consumer.Result;
        }

        private static TResult Owned<T, V, TResult>(T[] array, ITransmutation<T, V> transform, Consumer<V, TResult> consumer)
        {
            try
            {
                foreach (var item in array)
                {
                    var processNextResult = transform.OwnedProcessNext(item, out var u);
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

        private static TResult Owned<T, V, TResult>(List<T> lst, ITransmutation<T, V> transform, Consumer<V, TResult> consumer)
        {
            try
            {
                foreach (var item in lst)
                {
                    var processNextResult = transform.OwnedProcessNext(item, out var u);
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

        private static TResult Pipeline<T, V, TResult>(T[] array, ITransmutation<T, V> transform, Consumer<V, TResult> consumer)
        {
            var activity = transform.Compose(consumer);
            try
            {
                foreach (var item in array)
                {
                    var processNextResult = activity.ProcessNext(item);
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

        private static TResult Pipeline<T, V, TResult>(List<T> lst, ITransmutation<T, V> transform, Consumer<V, TResult> consumer)
        {
            var activity = transform.Compose(consumer);
            try
            {
                foreach (var item in lst)
                {
                    var processNextResult = activity.ProcessNext(item);
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

        private static TResult Pipeline<T, V, TResult>(IEnumerable<T> e, ITransmutation<T, V> transform, Consumer<V, TResult> consumer)
        {
            var activity = transform.Compose(consumer);
            try
            {
                foreach (var item in e)
                {
                    var processNextResult = activity.ProcessNext(item);
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
