using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class DistinctImpl<T> : Transmutation<T, T>
        {
            private readonly IEqualityComparer<T> comparer;

            public DistinctImpl(IEqualityComparer<T> comparer) =>
                this.comparer = comparer;

            public override ConsumerActivity<T, V> Compose<V>(IOutOfBand consumer, ConsumerActivity<T, V> activity) =>
                new Activity<V>(comparer, activity);

            private class Activity<V> : ConsumerActivity<T, T, V>
            {
                private readonly HashSet<T> seen;

                public Activity(IEqualityComparer<T> comparer, ConsumerActivity<T, V> next)
                    : base(next)
                {
                    this.seen = new HashSet<T>(comparer);
                }

                public override bool ProcessNext(T input) =>
                    seen.Add(input) && next.ProcessNext(input);
            }
        }

        internal static Consumable<TSource> Distinct<TSource>(
            IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");

            return Utils.PushTransform(source, new DistinctImpl<TSource>(comparer ?? EqualityComparer<TSource>.Default));
        }

        internal static Consumable<TSource> Distinct<TSource>(
            IEnumerable<TSource> source
        )
        {
            return Distinct(source, null);
        }
}
}
