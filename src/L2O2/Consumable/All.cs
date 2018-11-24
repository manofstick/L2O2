using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        class AllImpl<T> : Consumer<T, bool>
        {
            private Func<T, bool> selector;

            public AllImpl(Func<T, bool> selector)
                : base(true)
            {
                this.selector = selector;
            }

            public override bool ProcessNext(T input, ref bool result)
            {
                if (!selector(input))
                {
                    result = false;
                    StopFurtherProcessing();
                }
                return true; /*ignored*/
            }
        }

        internal static bool All<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return Utils.Consume(source, new AllImpl<TSource>(predicate));
        }
    }
}
