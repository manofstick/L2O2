using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal abstract class SelectImpl<T> : ITransmutation<T>
        {
            public abstract Consumable<U> AddSelector<U>(Consumable<T> consumer, Func<T, U> t2u);
        }

        internal class SelectImpl<T, U> : SelectImpl<U>, ITransmutation<T, U>
        {
            public SelectImpl(Func<T, U> selector)
            {
                this.Selector = selector;
            }

            public Func<T, U> Selector { get; }

            public override Consumable<V> AddSelector<V>(Consumable<U> consumer, Func<U, V> u2v)
            {
                return consumer.ReplaceTail(new SelectImpl<T, U, V>(Selector, u2v));
            }

            public ConsumerActivity<T, V> Compose<V>(IOutOfBand consumer, ConsumerActivity<U, V> activity)
            {
                return new Activity<V>(Selector, activity);
            }

            public bool OwnedProcessNext(T t, out U u)
            {
                u = Selector(t);
                return true;
            }

            public bool TryOwn()
            {
                return true;
            }

            private class Activity<V> : ConsumerActivity<T, U, V>
            {
                private readonly Func<T, U> selector;

                public Activity(Func<T, U> selector, ConsumerActivity<U, V> next)
                    : base(next)
                {
                    this.selector = selector;
                }

                public override bool ProcessNext(T input)
                {
                    return next.ProcessNext(selector(input));
                }
            }
        }

        internal class SelectImpl<T, U, V> : SelectImpl<T, V>
        {
            private readonly Func<T, U> t2u;
            private readonly Func<U, V> u2v;

            public SelectImpl(Func<T, U> t2u, Func<U, V> u2v)
                : base (t => u2v(t2u(t)))
            {
                this.t2u = t2u;
                this.u2v = u2v;
            }

            public override Consumable<W> AddSelector<W>(Consumable<V> consumer, Func<V, W> v2w)
            {
                return consumer.ReplaceTail(new SelectImpl<T, U, V, W>(t2u, u2v, v2w));
            }
        }

        internal class SelectImpl<T, U, V, W> : SelectImpl<T, W>
        {
            private readonly Func<T, U> t2u;
            private readonly Func<U, V> u2v;
            private readonly Func<V, W> v2w;

            public SelectImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w)
                : base(t => v2w(u2v(t2u(t))))
            {
                this.t2u = t2u;
                this.u2v = u2v;
                this.v2w = v2w;
            }

            public override Consumable<X> AddSelector<X>(Consumable<W> consumer, Func<W, X> w2x)
            {
                return consumer.ReplaceTail(new SelectImpl<T, U, V, W, X>(t2u, u2v, v2w, w2x));
            }
        }

        internal class SelectImpl<T, U, V, W, X> : SelectImpl<T, X>
        {
            public SelectImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w, Func<W,X> w2x)
                : base(t => w2x(v2w(u2v(t2u(t)))))
            {}
        }

        private static SelectImpl<TSource, TResult> CreateSelect<TSource, TResult>(Func<TSource, TResult> selector)
        {
            return new SelectImpl<TSource, TResult>(selector);
        }

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
