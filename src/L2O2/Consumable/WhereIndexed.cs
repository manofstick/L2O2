using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class WhereIndexedImpl<T> : Transmutation<T, T>
        {
            private readonly int initialThreadId = Environment.CurrentManagedThreadId;
            private bool owned = false;
            private int index = 0;

            internal readonly Func<T, int, bool> predicate;

            public WhereIndexedImpl(Func<T, int, bool> predicate) =>
                this.predicate = predicate;

            public override ConsumerActivity<T, V, Result> Compose<V, Result>(IOutOfBand consumer, ConsumerActivity<T, V, Result> activity) =>
                new Activity<V, Result>(predicate, activity);

            public override bool TryOwn()
            {
                if (initialThreadId == Environment.CurrentManagedThreadId && !owned)
                {
                    owned = true;
                    return true;
                }
                return false;
            }

            public override bool OwnedProcessNext(T tin, out T tout) =>
                predicate(tout = tin, index++);

            private class Activity<V, Result> : ConsumerActivity<T, T, V, Result>
            {
                private readonly Func<T, int, bool> predicate;

                private int index;

                public Activity(Func<T, int, bool> predicate, ConsumerActivity<T, V, Result> next)
                    : base(next)
                {
                    this.predicate = predicate;
                }

                public override bool ProcessNext(T input, ref Result result) =>
                    predicate(input, index++) && next.ProcessNext(input, ref result);
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
