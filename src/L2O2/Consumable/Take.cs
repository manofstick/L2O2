using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        sealed class TakeImpl<T> : Transmutation<T, T>
        {
            internal readonly int count;

            public TakeImpl(int count) =>
                this.count = count;

            public override Chain<T, V> Compose<V>(Chain<T, V> activity) =>
                new Activity<V>(count, activity);

            sealed class Activity<V> : Activity<T, T, V>
            {
                private readonly int count;

                private int index;

                public Activity(int count, Chain<T, V> next) : base(next) =>
                    (this.count, index) = (count, 0);

                public override ProcessNextResult ProcessNext(T input)
                {
                    if (index >= count)
                        return Stop;

                    checked
                    {
                        index++;
                    }

                    return (index >= count) ? Stop | Next(input) : Next(input);
                }
            }
        }

        internal static IEnumerable<TSource> Take<TSource>(
            this IEnumerable<TSource> source,
            int count)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return Utils.PushTransform(source, new TakeImpl<TSource>(count));
        }
    }
}
