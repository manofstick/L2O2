﻿using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class WhereImpl<T> : ITransmutation<T, T>
        {
            private readonly Func<T, bool> predicate;

            internal Func<T, bool> Predicate => predicate;

            public WhereImpl(Func<T, bool> predicate) =>
                this.predicate = predicate;

            public Chain<T, U> Compose<U>(Chain<T, U> activity) =>
                new Activity<U>(Predicate, activity);

            public bool IsStateless() => true;

            public ProcessNextResult ProcessNextStateless(T tin, out T tout) =>
                Predicate(tout = tin) ? ProcessNextResult.Flow :  ProcessNextResult.Filter;

            sealed class Activity<U> : Activity<T, T, U>
            {
                private readonly Func<T, bool> predicate;

                public Activity(Func<T, bool> predicate, Chain<T, U> next) : base(next) =>
                    this.predicate = predicate;

                public override ProcessNextResult ProcessNext(T input) =>
                    predicate(input) ? Next(input) : Filter;
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

        //internal class WhereImpl<T, U, V, W, X> : WhereImpl<T, X>
        //{
        //    public WhereImpl(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w, Func<W, X> w2x)
        //        : base(t => w2x(v2w(u2v(t2u(t)))))
        //    { }
        //}

        internal static Consumable<TSource> Where<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("selector");

            return Utils.PushTransform(source, new WhereImpl<TSource>(predicate));
        }
    }
}
