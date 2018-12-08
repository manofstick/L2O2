using System;
using System.Collections;
using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    static partial class Impl
    {
        public static List<V> ToList<T, U, V>(Consumable<V> consumable, T[] array, IComposition<T, U, V> composition)
        {
            if (composition.First == IdentityTransform<T>.Instance)
            {
                switch (composition.Second)
                {
                    case SelectImpl<T, V> select: return ToList(array, select.Selector);
                }
            }
            return consumable.Consume(new ToListImpl<V>());
        }

        public static List<V> ToList<T, U, V>(Consumable<V> consumable, List<T> array, IComposition<T, U, V> composition)
        {
            if (composition.First == IdentityTransform<T>.Instance)
            {
                switch (composition.Second)
                {
                    case SelectImpl<T, V> select: return ToList(array, select.Selector);
                }
            }
            return consumable.Consume(new ToListImpl<V>());
        }

        public static List<V> ToList<T, U, V>(Consumable<V> consumable, IEnumerable<T> enumerable, IComposition<T, U, V> composition)
        {
            if (composition.First == IdentityTransform<T>.Instance)
            {
                switch (composition.Second)
                {
                    case SelectImpl<T, V> select: return ToList(enumerable, select.Selector);
                }
            }
            return consumable.Consume(new ToListImpl<V>());
        }

        /// <summary>
        /// This, and derived classes, should *only* for use in the constructor of a List<V>.
        /// We are *expecting* the implementation of List to use the ICollection<> interface, which
        /// it does (but we do provide a IEnumerable implementation as a less efficient redundancy)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        private abstract class ToListSelectorBase<T, V> : ICollection<V>
        {
            abstract public int Count { get; }
            abstract public IEnumerator<V> GetEnumerator();
            abstract public void CopyTo(V[] array, int arrayIndex);

            bool ICollection<V>.IsReadOnly => true;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            void ICollection<V>.Add(V item) => throw new NotImplementedException();
            void ICollection<V>.Clear() => throw new NotImplementedException();
            bool ICollection<V>.Contains(V item) => throw new NotImplementedException();
            bool ICollection<V>.Remove(V item) => throw new NotImplementedException();
        }

        private class ArraySelectorToList<T, V> : ToListSelectorBase<T, V>
        {
            readonly T[] source;
            readonly Func<T, V> selector;

            public ArraySelectorToList(T[] array, Func<T, V> selector) =>
                (source, this.selector) = (array, selector);

            override public int Count => source.Length;

            override public void CopyTo(V[] array, int arrayIndex)
            {
                for (var i = 0; i < array.Length; ++i)
                    array[arrayIndex + i] = selector(source[i]);
            }

            override public IEnumerator<V> GetEnumerator() => Impl.GetEnumerator(source, selector);
        }

        private class ListSelectorToList<T, V> : ToListSelectorBase<T, V>
        {
            readonly List<T> source;
            readonly Func<T, V> selector;

            public ListSelectorToList(List<T> lst, Func<T, V> selector) =>
                (source, this.selector) = (lst, selector);

            override public int Count => source.Count;

            override public void CopyTo(V[] array, int arrayIndex)
            {
                for (var i = 0; i < source.Count; ++i)
                    array[arrayIndex + i] = selector(source[i]);
            }

            override public IEnumerator<V> GetEnumerator() => Impl.GetEnumerator(source, selector);
        }

        static private List<V> ToList<T, V>(IEnumerable<T> enumerable, Func<T, V> selector)
        {
            var l = new List<V>();
            foreach (var item in enumerable)
                l.Add(selector(item));
            return l;
        }

        static private List<V> ToList<T, V>(T[] array, Func<T, V> selector) =>
            new List<V>(new ArraySelectorToList<T, V>(array, selector));

        static private List<V> ToList<T, V>(List<T> list, Func<T, V> selector) =>
            new List<V>(new ListSelectorToList<T, V>(list, selector));
    }
}
