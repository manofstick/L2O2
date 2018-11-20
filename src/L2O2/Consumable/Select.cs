using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal abstract class SelectImpl<T> : ISeqTransform<T>
        {
            public abstract Consumable<U> AddSelector<U>(Consumable<T> consumer, Func<T, U> t2u);
        }

        internal class SelectImpl<T, U> : SelectImpl<U>, ISeqTransform<T, U>
        {
            internal readonly Func<T, U> selector;

            public SelectImpl(Func<T, U> selector)
            {
                this.selector = selector;
            }

            public override Consumable<V> AddSelector<V>(Consumable<U> consumer, Func<U, V> u2v)
            {
                return consumer.ReplaceTail(new SelectImpl<T, U, V>(selector, u2v));
            }

            public SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<U, V> activity)
            {
                return new Activity<V>(selector, activity);
            }

            public bool OwnedProcessNext(T t, out U u)
            {
                u = selector(t);
                return true;
            }

            public bool TryOwn()
            {
                return true;
            }

            private class Activity<V> : SeqConsumerActivity<T, V>
            {
                private readonly Func<T, U> selector;
                private readonly SeqConsumerActivity<U, V> next;

                public Activity(Func<T, U> selector, SeqConsumerActivity<U, V> next)
                {
                    this.selector = selector;
                    this.next = next;
                }

                public override bool ProcessNext(T input)
                {
                    return next.ProcessNext(selector(input));
                }
            }
        }

        internal class SelectImpl<T, U, V> : SelectImpl<T, V>
        {
            private Func<T, U> t2u;
            private Func<U, V> u2v;

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
            private Func<T, U> t2u;
            private Func<U, V> u2v;
            private Func<V, W> v2w;

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

        internal static Consumable<TResult> Select<TSource, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            switch (source)
            {
                case Consumable<TSource> consumable:
                    switch (consumable.TailTransform)
                    {
                        case SelectImpl<TSource> select:
                            return select.AddSelector(consumable, selector);

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            return Utils.PushTransform(source, new SelectImpl<TSource, TResult>(selector));
        }
    }

}
