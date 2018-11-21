﻿using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class SelectIImpl<T, U> : Transmutation<T, U>
        {
            private readonly int initialThreadId = Environment.CurrentManagedThreadId;
            private bool owned = false;
            private int index = 0;

            internal readonly Func<T, int, U> selector;

            public SelectIImpl(Func<T, int, U> selector)
            {
                this.selector = selector;
            }

            public override ConsumerActivity<T, V> Compose<V>(IOutOfBand consumer, ConsumerActivity<U, V> activity)
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

            public override bool OwnedProcessNext(T t, out U u)
            {
                u = selector(t, index++);
                return true;
            }

            private class Activity<V> : ConsumerActivity<T, V>
            {
                private readonly Func<T, int, U> selector;
                private readonly ConsumerActivity<U, V> next;

                private int index;

                public Activity(Func<T, int, U> selector, ConsumerActivity<U, V> next)
                {
                    this.selector = selector;
                    this.next = next;
                }

                public override bool ProcessNext(T input)
                {
                    return next.ProcessNext(selector(input, index++));
                }
            }
        }

        internal static Consumable<TResult> Select<TSource, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return Utils.PushTransform(source, new SelectIImpl<TSource, TResult>(selector));
        }
    }
}
