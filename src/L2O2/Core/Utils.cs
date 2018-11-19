using System;
using System.Collections.Generic;

namespace L2O2.Core
{
    static class Utils
    {
        internal static IConsumableSeq<T> OfSeq<T>(this System.Collections.Generic.IEnumerable<T> e)
        {
            switch (e)
            {
                case IConsumableSeq<T> consumable:
                    return consumable;

                case T[] array:
                    return new ArrayEnumerable<T, T, T>(array, IdentityTransform<T>.Instance, IdentityTransform<T>.Instance);

                case List<T> list:
                    return new ListEnumerable<T, T, T>(list, IdentityTransform<T>.Instance, IdentityTransform<T>.Instance);

                default:
                    return new EnumerableEnumerable<T, T, T>(e, IdentityTransform<T>.Instance, IdentityTransform<T>.Instance);
            }
        }

        internal static IConsumableSeq<U> PushTransform<T,U>(this IEnumerable<T> e, ISeqTransform<T,U> transform)
        {
            switch (e)
            {
                case IConsumableSeq<T> consumable:
                    return consumable.Transform(transform);

                case T[] array:
                    return new ArrayEnumerable<T, T, U>(array, IdentityTransform<T>.Instance, transform);

                case List<T> list:
                    return new ListEnumerable<T, T, U>(list, IdentityTransform<T>.Instance, transform);

                default:
                    return new EnumerableEnumerable<T, T, U>(e, IdentityTransform<T>.Instance, transform);
            }
        }

        internal class EmptyEnumerator<T>
        {
            private static IEnumerator<T> Create() { yield break; }
            public static IEnumerator<T> Instance = Create();
        }


    }
}
