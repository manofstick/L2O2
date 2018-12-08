using System;
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
                if (array.Length <= MaxLengthToAvoidPipelineCreationCost && transform.IsStateless())
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
                if (lst.Count <= MaxLengthToAvoidPipelineCreationCost && transform.IsStateless())
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

        class SelectManyInnerConsumer<T> : Consumer<T, ProcessNextResult>
        {
            private readonly Chain<T> chainT;

            public SelectManyInnerConsumer(Chain<T> chainT) : base(ProcessNextResult.OK) =>
                this.chainT = chainT;

            public override ProcessNextResult ProcessNext(T input)
            {
                var rc = chainT.ProcessNext(input);
                Result = rc;
                return rc;
            }
        }

        class SelectManyOuterConsumer<T> : Consumer<IEnumerable<T>, ChainEnd>
        {
            private readonly Chain<T> chainT;
            private SelectManyInnerConsumer<T> inner;

            private SelectManyInnerConsumer<T> GetInnerConsumer()
            {
                if (inner == null)
                    inner = new SelectManyInnerConsumer<T>(chainT);
                return inner;
            }

            public SelectManyOuterConsumer(Chain<T> chainT) : base(default(ChainEnd)) =>
                this.chainT = chainT;

            public override ProcessNextResult ProcessNext(IEnumerable<T> input)
            {
                var rc = ProcessNextResult.OK;
                switch (input)
                {
                    case Consumable<T> consumable:
                        rc = consumable.Consume(GetInnerConsumer());
                        break;

                    default:
                        foreach (var item in input)
                        {
                            rc = chainT.ProcessNext(item);
                            if (rc.IsHalted())
                                break;

                        }
                        break;
                }
                return rc == ProcessNextResult.HaltedConsumer ? ProcessNextResult.HaltedConsumer : ProcessNextResult.OK;
            }
        }

        public static Result Consume<T, U, V, Result>(Consumable<IEnumerable<T>> e, IComposition<T, U, V> composition, Consumer<V, Result> consumer)
        {
            e.Consume(new SelectManyOuterConsumer<T>(composition.Composed.Compose(consumer)));
            return consumer.Result;
        }

        class SelectManyInnerConsumer<TSource, TCollection, T> : Consumer<TCollection, ProcessNextResult>
        {
            private readonly Chain<T> chainT;
            private readonly Func<TSource, TCollection, T> resultSelector;

            public TSource Source { get; set; }

            public SelectManyInnerConsumer(Func<TSource, TCollection, T> resultSelector, Chain<T> chainT) : base(ProcessNextResult.OK) =>
                (this.chainT, this.resultSelector) = (chainT, resultSelector);

            public override ProcessNextResult ProcessNext(TCollection input)
            {
                var rc = chainT.ProcessNext(resultSelector(Source, input));
                Result = rc;
                return rc;
            }
        }

        class SelectManyOuterConsumer<TSource, TCollection, T> : Consumer<(TSource, IEnumerable<TCollection>), ChainEnd>
        {
            Func<TSource, TCollection, T> resultSelector;
            private readonly Chain<T> chainT;
            private SelectManyInnerConsumer<TSource, TCollection, T> inner;

            private SelectManyInnerConsumer<TSource, TCollection, T> GetInnerConsumer()
            {
                if (inner == null)
                    inner = new SelectManyInnerConsumer<TSource, TCollection, T>(resultSelector, chainT);
                return inner;
            }

            public SelectManyOuterConsumer(Func<TSource, TCollection, T> resultSelector, Chain<T> chainT) : base(default(ChainEnd)) =>
                (this.chainT, this.resultSelector) = (chainT, resultSelector);

            public override ProcessNextResult ProcessNext((TSource, IEnumerable<TCollection>) input)
            {
                var rc = ProcessNextResult.OK;
                switch (input.Item2)
                {
                    case Consumable<TCollection> consumable:
                        var consumer = GetInnerConsumer();
                        consumer.Source = input.Item1;
                        rc = consumable.Consume(consumer);
                        break;

                    default:
                        foreach (var item in input.Item2)
                        {
                            rc = chainT.ProcessNext(resultSelector(input.Item1, item));
                            if (rc.IsHalted())
                                break;

                        }
                        break;
                }
                return rc == ProcessNextResult.HaltedConsumer ? ProcessNextResult.HaltedConsumer : ProcessNextResult.OK;
            }
        }

        public static Result Consume<TSource, TCollection, T, U, V, Result>(Consumable<(TSource, IEnumerable<TCollection>)> e, Func<TSource, TCollection, T> resultSelector, IComposition<T, U, V> composition, Consumer<V, Result> consumer)
        {
            e.Consume(new SelectManyOuterConsumer<TSource, TCollection, T>(resultSelector, composition.Composed.Compose(consumer)));
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
                    var processNextResult = transform.ProcessNextStateless(item, out var u);
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
                    var processNextResult = transform.ProcessNextStateless(item, out var u);
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
