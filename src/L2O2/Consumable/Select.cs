﻿using L2O2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace L2O2
{
    public static partial class Consumable
    {
        private class SelectImpl<T, U> : ISeqTransform<T, U>
        {
            private readonly Func<T, U> selector;

            public SelectImpl(Func<T, U> selector)
            {
                this.selector = selector;
            }

            public SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<U, V> activity)
            {
                return new Activity<V>(selector, activity);
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

        internal static IConsumableSeq<TResult> Select<TSource, TResult>(
            this IConsumableSeq<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.Transform(new SelectImpl<TSource, TResult>(selector));
        }
    }
}
