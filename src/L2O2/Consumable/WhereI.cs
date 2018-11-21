using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class WhereIImpl<T> : SeqTransform<T, T>
        {
            private readonly int initialThreadId = Environment.CurrentManagedThreadId;
            private bool owned = false;
            private int index = 0;

            internal readonly Func<T, int, bool> predicate;

            public WhereIImpl(Func<T, int, bool> predicate) =>
                this.predicate = predicate;

            public override SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<T, V> activity) =>
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

            public override bool OwnedProcessNext(T tin, out T tout) =>
                predicate(tout = tin, index++);

            private class Activity<V> : SeqConsumerActivity<T, V>
            {
                private readonly Func<T, int, bool> predicate;
                private readonly SeqConsumerActivity<T, V> next;

                private int index;

                public Activity(Func<T, int, bool> predicate, SeqConsumerActivity<T, V> next)
                {
                    this.predicate = predicate;
                    this.next = next;
                }

                public override bool ProcessNext(T input) =>
                    predicate(input, index++) && next.ProcessNext(input);
            }
        }

        internal static Consumable<TSource> Where<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, int, bool> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return Utils.PushTransform(source, new WhereIImpl<TSource>(selector));
        }
    }
}
