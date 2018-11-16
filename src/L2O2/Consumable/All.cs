using L2O2.Core;
using System;

namespace L2O2
{
    public static partial class Consumable
    {
        class AllImpl<T> : SeqConsumer<T, bool>
        {
            private Func<T, bool> selector;

            public AllImpl(Func<T, bool> selector)
                : base(true)
            {
                this.selector = selector;
            }

            public override bool ProcessNext(T input)
            {
                if (!selector(input))
                {
                    Result = false;
                    StopFurtherProcessing();
                }
                return true; /*ignored*/
            }
        }

        internal static bool All<TSource>(
            this IConsumableSeq<TSource> source,
            Func<TSource, bool> selector)
        {
            return source.Consume(() => new AllImpl<TSource>(selector));
        }
    }
}
