using System;
using System.Collections.Generic;

namespace L2O2.Core
{
    static class Utils
    {
        internal static IConsumableSeq<T> OfSeq<T>(this IEnumerable<T> e)
        {
            switch(e)
            {
                case IConsumableSeq<T> consumable:
                    return consumable;

                case T[] array:
                    return new ArrayEnumerable<T, T>(array, IdentityTransform<T>.Instance);

                case List<T> list:
                    return new ListEnumerable<T, T>(list, IdentityTransform<T>.Instance);

                default:
                    return new EnumerableEnumerable<T, T>(e, IdentityTransform<T>.Instance);
            }
        }
    }
}
