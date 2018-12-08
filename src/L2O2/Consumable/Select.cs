using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal abstract class SelectImpl<T>
        {
            public abstract Consumable<U> AddSelector<U>(Consumable<T> consumer, Func<T, U> t2u);
        }

        internal class SelectImpl<T, U> : SelectImpl<U>, ITransmutation<T, U>
        {
            private readonly Func<T, U> t2u;

            public SelectImpl(Func<T, U> selector) => 
                t2u = selector;

            public Func<T, U> Selector { get => t2u; }

            public override Consumable<V> AddSelector<V>(Consumable<U> consumer, Func<U, V> u2v) =>
                consumer.ReplaceTail(new SelectImpl<T, U, V>(t2u, u2v));

            public Chain<T, V> Compose<V>(Chain<U, V> activity) =>
                new Activity<V>(t2u, activity);

            public bool IsStateless() => true;

            public ProcessNextResult ProcessNextStateless(T t, out U u)
            {
                u = Selector(t);
                return ProcessNextResult.OK;
            }

            sealed class Activity<V> : Activity<T, U, V>
            {
                private readonly Func<T, U> selector;

                public Activity(Func<T, U> selector, Chain<U, V> next) : base(next) =>
                    this.selector = selector;

                public override ProcessNextResult ProcessNext(T input) =>
                    Next(selector(input));
            }
        }

        sealed class SelectImpl<T, U, V> : SelectImpl<T, V>
        {
            private readonly Func<T, U> t2u;
            private readonly Func<U, V> u2v;

            public SelectImpl(Func<T, U> t2u, Func<U, V> u2v) : base(t => u2v(t2u(t))) =>
                (this.t2u, this.u2v) = (t2u, u2v);

            public override Consumable<W> AddSelector<W>(Consumable<V> consumer, Func<V, W> v2w) =>
                consumer.ReplaceTail(new SelectImpl<T, U, V, W>(t2u, u2v, v2w));
        }

        sealed class SelectImpl<T, U, V, W> : SelectImpl<T, W>
        {
            private readonly Func<T, U> t2u;
            private readonly Func<U, V> u2v;
            private readonly Func<V, W> v2w;

            public SelectImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w) : base(t => v2w(u2v(t2u(t)))) =>
                (this.t2u, this.u2v, this.v2w) = (t2u, u2v, v2w);

            public override Consumable<X> AddSelector<X>(Consumable<W> consumer, Func<W, X> w2x) =>
                consumer.ReplaceTail(new SelectImpl<T, U, V, W, X>(t2u, u2v, v2w, w2x));
        }

        sealed class SelectImpl<T, U, V, W, X> : SelectImpl<T, X>
        {
            public SelectImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w, Func<W,X> w2x)
                : base(t => w2x(v2w(u2v(t2u(t)))))
            {}
        }

        private static SelectImpl<TSource, TResult> CreateSelect<TSource, TResult>(Func<TSource, TResult> selector) =>
            new SelectImpl<TSource, TResult>(selector);

        internal static Consumable<TResult> Select<TSource, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            switch (source)
            {
                case Consumable<TSource> consumable:
                    switch (consumable.Tail)
                    {
                        case SelectImpl<TSource> select:
                            return select.AddSelector(consumable, selector);

                        default:
                            break;
                    }
                    return consumable.AddTail(CreateSelect(selector));

                default:
                    return Utils.CreateConsumable(source, CreateSelect(selector));
            }
        }
    }

}
