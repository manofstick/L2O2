using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        sealed class WhereIndexedImpl<T> : Transmutation<T, T>
        {
            private readonly int initialThreadId = Environment.CurrentManagedThreadId;
            private bool owned = false;
            private int index = 0;

            internal readonly Func<T, int, bool> predicate;

            public WhereIndexedImpl(Func<T, int, bool> predicate) =>
                this.predicate = predicate;

            public override Chain<T, V> Compose<V>(Chain<T, V> activity) =>
                new Activity<V>(predicate, activity);

            public override bool TryOwn()
            {
                if (initialThreadId == Environment.CurrentManagedThreadId && !owned)
                {
                    owned = true;
                    return true;
                }
                return false;
            }

            public override ProcessNextResult OwnedProcessNext(T tin, out T tout) =>
                predicate(tout = tin, index++) ? ProcessNextResult.OK : ProcessNextResult.Filtered;

            sealed class Activity<V> : Activity<T, T, V>
            {
                private readonly Func<T, int, bool> predicate;

                private int index;

                public Activity(Func<T, int, bool> predicate, Chain<T, V> next)
                    : base(next)
                {
                    this.predicate = predicate;
                }

                public override ProcessNextResult ProcessNext(T input) =>
                    predicate(input, index++) ? next.ProcessNext(input) : ProcessNextResult.Filtered;
            }
        }

        internal static Consumable<TSource> Where<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, int, bool> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return Utils.PushTransform(source, new WhereIndexedImpl<TSource>(selector));
        }
    }
}
