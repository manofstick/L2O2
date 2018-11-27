using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        class SinglePredicateImpl<T> : Consumer<T, T>
        {
            private readonly Func<T, bool> predicate;
            private bool found;

            public SinglePredicateImpl(Func<T, bool> predicate)
                : base(default(T))
            {
                this.predicate = predicate;
                found = false;
            }

            public override bool ProcessNext(T input, ref Status<T> result)
            {
                if (predicate(input))
                {
                    if (found)
                        throw new InvalidOperationException("Sequence contained multiple elements");

                    found = true;
                    result.Value = input;
                }

                return true; /*ignored*/
            }

            public override void ChainComplete(ref T result)
            {
                if (!found)
                    throw new InvalidOperationException("Sequence was empty");

                base.ChainComplete(ref result);
            }
        }

        internal static TSource Single<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("source");

            return Utils.Consume(source, new SinglePredicateImpl<TSource>(predicate));
        }
    }
}
