using L2O2.Core;
using System;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class WhereImpl<T> : ISeqTransform<T, T>
        {
            internal readonly Func<T, bool> selector;

            public WhereImpl(Func<T, bool> selector)
            {
                this.selector = selector;
            }

            public SeqConsumerActivity<T, U> Compose<U>(ISeqConsumer consumer, SeqConsumerActivity<T, U> activity)
            {
                return new Activity<U>(selector, activity);
            }

            public bool OwnedProcessNext(T tin, out T tout)
            {
                if (selector(tin))
                {
                    tout = tin;
                    return true;
                }
                else
                {
                    tout = default(T);
                    return false;
                }
            }

            virtual public bool TryAggregate<U>(ISeqTransform<T, U> next, out ISeqTransform<T, U> composite)
            {
                composite = null;
                return false;
                //if (next is WhereImpl<U, V> u2v)
                //{
                //    composite = new WhereImpl<T, U, V>(selector, u2v.selector);
                //    return true;
                //}
                //composite = null;
                //return false;
            }

            public bool TryOwn()
            {
                return true;
            }

            private class Activity<U> : SeqConsumerActivity<T, U>
            {
                private readonly Func<T, bool> selector;
                private readonly SeqConsumerActivity<T, U> next;

                public Activity(Func<T, bool> selector, SeqConsumerActivity<T, U> next)
                {
                    this.selector = selector;
                    this.next = next;
                }

                public override bool ProcessNext(T input)
                {
                    return selector(input) && next.ProcessNext(input);
                }
            }
        }

        //internal class WhereImpl<T, U, V> : WhereImpl<T, V>
        //{
        //    private Func<T, U> t2u;
        //    private Func<U, V> u2v;

        //    public WhereImpl(Func<T, U> t2u, Func<U, V> u2v)
        //        : base(t => u2v(t2u(t)))
        //    {
        //        this.t2u = t2u;
        //        this.u2v = u2v;
        //    }

        //    public override bool TryAggregate<W>(ISeqTransform<V, W> next, out ISeqTransform<T, W> composite)
        //    {
        //        if (next is WhereImpl<V, W> v2w)
        //        {
        //            composite = new WhereImpl<T, U, V, W>(t2u, u2v, v2w.selector);
        //            return true;
        //        }
        //        composite = null;
        //        return false;
        //    }
        //}

        //internal class WhereImpl<T, U, V, W> : WhereImpl<T, W>
        //{
        //    private Func<T, U> t2u;
        //    private Func<U, V> u2v;
        //    private Func<V, W> v2w;

        //    public WhereImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w)
        //        : base(t => v2w(u2v(t2u(t))))
        //    {
        //        this.t2u = t2u;
        //        this.u2v = u2v;
        //        this.v2w = v2w;
        //    }

        //    public override bool TryAggregate<X>(ISeqTransform<W, X> next, out ISeqTransform<T, X> composite)
        //    {
        //        if (next is WhereImpl<W, X> w2x)
        //        {
        //            composite = new WhereImpl<T, U, V, W, X>(t2u, u2v, v2w, w2x.selector);
        //            return true;
        //        }
        //        composite = null;
        //        return false;
        //    }
        //}

        //internal class WhereImpl<T, U, V, W, X> : WhereImpl<T, X>
        //{
        //    public WhereImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w, Func<W, X> w2x)
        //        : base(t => w2x(v2w(u2v(t2u(t)))))
        //    { }
        //}

        internal static IConsumableSeq<TSource> Where<TSource>(
            this IConsumableSeq<TSource> source,
            Func<TSource, bool> selector)
        {
            return source.Transform(new WhereImpl<TSource>(selector));
        }
    }
}
