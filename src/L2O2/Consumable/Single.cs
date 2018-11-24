using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        class SingleImpl<T> : Consumer<T, T>
        {
            private bool found;

            public SingleImpl()
                : base(default(T))
            {
                found = false;
            }

            public override bool ProcessNext(T input, ref T result)
            {
                if (found)
                    throw new InvalidOperationException("Sequence contained multiple elements");

                found = true;
                result = input;

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
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return Utils.Consume(source, new SingleImpl<TSource>());
        }
    }
}
