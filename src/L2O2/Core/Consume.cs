using System.Collections.Generic;

namespace L2O2.Core
{
    static partial class Impl
    {
        public static Result Consume<T, U, V, Result>(T[] array, IComposition<T, U, V> composition, Consumer<V, Result> consumer)
        {
            if (array.Length == 0)
                Empty(consumer);
            else
            {
                var transform = composition.Composed;

                const int MaxLengthToAvoidPipelineCreationCost = 5;
                if (array.Length <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
                    Owned(array, transform, consumer);
                else
                    Pipeline(array, composition.Composed.Compose(consumer));
            }
            return consumer.Result;
        }

        public static Result Consume<T, U, V, Result>(List<T> lst, IComposition<T, U, V> composition, Consumer<V, Result> consumer)
        {
            if (lst.Count == 0)
                Empty(consumer);
            else
            {
                var transform = composition.Composed;

                const int MaxLengthToAvoidPipelineCreationCost = 5;
                if (lst.Count <= MaxLengthToAvoidPipelineCreationCost && transform.TryOwn())
                    Owned(lst, transform, consumer);
                else
                    Pipeline(lst, composition.Composed.Compose(consumer));
            }
            return consumer.Result;
        }

        public static Result Consume<T, U, V, Result>(IEnumerable<T> e, IComposition<T, U, V> composition, Consumer<V, Result> consumer)
        {
            Pipeline(e, composition.Composed.Compose(consumer));
            return consumer.Result;
        }

        private static void Empty(Chain consumer)
        {
            try { consumer.ChainComplete(); }
            finally { consumer.ChainDispose(); }
        }

        private static void Owned<T, V>(T[] array, ITransmutation<T, V> transform, Chain<V> finalLink)
        {
            try
            {
                foreach (var item in array)
                {
                    var processNextResult = transform.OwnedProcessNext(item, out var u);
                    if (processNextResult.IsOK())
                        processNextResult = finalLink.ProcessNext(u);

                    if (processNextResult.IsHalted())
                        break;
                }
                finalLink.ChainComplete();
            }
            finally
            {
                finalLink.ChainDispose();
            }
        }

        private static void Owned<T, V>(List<T> lst, ITransmutation<T, V> transform, Chain<V> finalLink)
        {
            try
            {
                foreach (var item in lst)
                {
                    var processNextResult = transform.OwnedProcessNext(item, out var u);
                    if (processNextResult.IsOK())
                        processNextResult = finalLink.ProcessNext(u);

                    if (processNextResult.IsHalted())
                        break;
                }
                finalLink.ChainComplete();
            }
            finally
            {
                finalLink.ChainDispose();
            }
        }

        private static void Pipeline<T>(T[] array, Chain<T> chain)
        {
            try
            {
                foreach (var item in array)
                {
                    var processNextResult = chain.ProcessNext(item);
                    if (processNextResult.IsHalted())
                        break;
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<T>(List<T> lst, Chain<T> chain)
        {
            try
            {
                foreach (var item in lst)
                {
                    var processNextResult = chain.ProcessNext(item);
                    if (processNextResult.IsHalted())
                        break;
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<T>(IEnumerable<T> e, Chain<T> chain)
        {
            try
            {
                foreach (var item in e)
                {
                    var processNextResult = chain.ProcessNext(item);
                    if (processNextResult.IsHalted())
                        break;
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

    }
}
