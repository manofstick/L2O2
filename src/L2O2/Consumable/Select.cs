using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class SelectImpl<T, U> : ISeqTransform<T, U>
        {
            internal readonly Func<T, U> selector;

            public SelectImpl(Func<T, U> selector)
            {
                this.selector = selector;
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

            virtual public bool TryAggregate<V>(ISeqTransform<U, V> next, out ISeqTransform<T, V> composite)
            {
                if (next is SelectImpl<U,V> u2v)
                {
                    composite = new SelectImpl<T,U,V>(selector, u2v.selector);
                    return true;
                }
                composite = null;
                return false;
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

            public override bool TryAggregate<W>(ISeqTransform<V, W> next, out ISeqTransform<T, W> composite)
            {
                if (next is SelectImpl<V, W> v2w)
                {
                    composite = new SelectImpl<T,U,V,W>(t2u,u2v,v2w.selector);
                    return true;
                }
                composite = null;
                return false;
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

            public override bool TryAggregate<X>(ISeqTransform<W, X> next, out ISeqTransform<T, X> composite)
            {
                if (next is SelectImpl<W, X> w2x)
                {
                    composite = new SelectImpl<T, U, V, W, X>(t2u, u2v, v2w, w2x.selector);
                    return true;
                }
                composite = null;
                return false;
            }
        }

        internal class SelectImpl<T, U, V, W, X> : SelectImpl<T, X>
        {
            public SelectImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w, Func<W,X> w2x)
                : base(t => w2x(v2w(u2v(t2u(t)))))
            {}
        }

        internal static IConsumableSeq<TResult> Select<TSource, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return Utils.PushTransform(source, new SelectImpl<TSource, TResult>(selector));
        }
    }

}
