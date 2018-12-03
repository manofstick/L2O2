﻿using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class SelectIndexedImpl<T, U> : Transmutation<T, U>
        {
            private readonly int initialThreadId = Environment.CurrentManagedThreadId;
            private bool owned = false;
            private int index = 0;

            internal readonly Func<T, int, U> selector;

            public SelectIndexedImpl(Func<T, int, U> selector)
            {
                this.selector = selector;
            }

            public override Chain<T, V> Compose<V>(Chain<U, V> activity)
            {
                return new Activity<V>(selector, activity);
            }

            public override bool TryOwn()
            {
                if (initialThreadId == Environment.CurrentManagedThreadId && !owned)
                {
                    owned = true;
                    return true;
                }
                return false;
            }

            public override ProcessNextResult OwnedProcessNext(T t, out U u)
            {
                u = selector(t, index++);
                return ProcessNextResult.OK;
            }

            private class Activity<V> : Activity<T, U, V>
            {
                private readonly Func<T, int, U> selector;

                private int index;

                public Activity(Func<T, int, U> selector, Chain<U, V> next)
                    : base(next)
                {
                    this.selector = selector;
                }

                public override ProcessNextResult ProcessNext(T input) =>
                    next.ProcessNext(selector(input, index++));
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
