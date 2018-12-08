using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        sealed class SelectIndexedImpl<T, U> : Transmutation<T, U>
        {
            internal readonly Func<T, int, U> selector;

            public SelectIndexedImpl(Func<T, int, U> selector) =>
                this.selector = selector;

            public override Chain<T, V> Compose<V>(Chain<U, V> activity) =>
                new Activity<V>(selector, activity);

            sealed class Activity<V> : Activity<T, U, V>
            {
                private readonly Func<T, int, U> selector;

                private int index;

                public Activity(Func<T, int, U> selector, Chain<U, V> next) : base(next) =>
                    this.selector = selector;

                public override ProcessNextResult ProcessNext(T input) =>
                    Next(selector(input, index++));
            }
        }

        internal static Consumable<TResult> Select<TSource, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return Utils.PushTransform(source, new SelectIndexedImpl<TSource, TResult>(selector));
        }
    }
}
