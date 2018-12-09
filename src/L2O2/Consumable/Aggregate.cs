using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        sealed class ReduceImpl<T> : Consumer<T, T>
        {
            private readonly Func<T, T, T> func;

            private bool first;

            public ReduceImpl(Func<T, T, T> func)
                : base(default(T))
            {
                this.first = true;
                this.func = func;
            }

            public override ProcessNextResult ProcessNext(T input)
            {
                if (first)
                {
                    first = false;
                    Result = input;
                }
                else
                {
                    Result = func(Result, input);
                }

                return Flow;
            }

            public override void ChainComplete()
            {
                if (first)
                    throw new InvalidOperationException();

                base.ChainComplete();
            }
        }

        sealed class AggregateImpl<T, TAccumulate, TResult> : Consumer<T, TResult>
        {
            private readonly Func<TAccumulate, T, TAccumulate> func;
            private readonly Func<TAccumulate, TResult> resultSelector;

            private TAccumulate accumulate;

            public AggregateImpl(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
                : base(default(TResult))
            {
                this.accumulate = seed;
                this.func = func;
                this.resultSelector = resultSelector;
            }

            public override ProcessNextResult ProcessNext(T input)
            {
                this.accumulate = func(accumulate, input);

                return Flow;
            }

            public override void ChainComplete()
            {
                Result = resultSelector(accumulate);

                base.ChainComplete();
            }
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return Utils.Consume(source, new AggregateImpl<TSource, TAccumulate, TResult>(seed, func, resultSelector));
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            return Aggregate(source, seed, func, x => x);
        }

        public static TSource Aggregate<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");

            return Utils.Consume(source, new ReduceImpl<TSource>(func));
        }
    }
}
