using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        sealed class DistinctImpl<T> : Transmutation<T, T>
        {
            private readonly IEqualityComparer<T> comparer;

            public DistinctImpl(IEqualityComparer<T> comparer) =>
                this.comparer = comparer;

            public override Chain<T, V> Compose<V>(Chain<T, V> activity) =>
                new Activity<V>(comparer, activity);

            sealed class Activity<V> : Activity<T, T, V>
            {
                private readonly HashSet<T> seen;

                public Activity(IEqualityComparer<T> comparer, Chain<T, V> next) : base(next) =>
                    seen = new HashSet<T>(comparer);

                public override ProcessNextResult ProcessNext(T input) =>
                    seen.Add(input) ? Next(input) : ProcessNextResult.Filter;
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
