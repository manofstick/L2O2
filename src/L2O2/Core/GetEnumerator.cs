using System;
using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    static partial class Impl
    {
        public static IEnumerator<V> GetEnumerator<T,U,V>(T[] array, IComposition<T, U, V> composition)
        {
            if (array.Length == 0)
                return Utils.EmptyEnumerator<V>.Instance;

            if (ReferenceEquals(composition.First, IdentityTransform<T>.Instance))
            {
                switch (composition.Second)
                {
                    case SelectImpl<T, V> select: return GetEnumerator(array, select.Selector);
                }
            }

            return ConsumableArrayEnumerator<T, V>.Create(array, composition.Composed);
        }

        public static IEnumerator<V> GetEnumerator<T, U, V>(List<T> lst, IComposition<T, U, V> composition)
        {
            if (lst.Count == 0)
                return Utils.EmptyEnumerator<V>.Instance;

            if (ReferenceEquals(composition.First, IdentityTransform<T>.Instance))
            {
                switch (composition.Second)
                {
                    case SelectImpl<T, V> select: return GetEnumerator(lst, select.Selector);
                }
            }

            return ConsumableListEnumerator<T, V>.Create(lst, composition.Composed);
        }

        public static IEnumerator<V> GetEnumerator<T, U, V>(IEnumerable<T> e, IComposition<T, U, V> composition)
        {
            if (ReferenceEquals(composition.First, IdentityTransform<T>.Instance))
            {
                switch (composition.Second)
                {
                    case SelectImpl<T, V> select: return GetEnumerator(e, select.Selector);
                }
            }

            return ConsumableEnumerableEnumerator<T, V>.Create(e, composition.Composed);
        }

        private static IEnumerator<V> GetEnumerator<T, V>(T[] array, Func<T, V> selector)
        {
            foreach (var item in array)
                yield return selector(item);
        }

        private static IEnumerator<V> GetEnumerator<T, V>(List<T> lst, Func<T, V> selector)
        {
            foreach (var item in lst)
                yield return selector(item);
        }

        private static IEnumerator<V> GetEnumerator<T, V>(IEnumerable<T> e, Func<T, V> selector)
        {
            foreach (var item in e)
                yield return selector(item);
        }

    }
}
