using L2O2.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal class ArraySelectorToList<T, V> : ICollection<V>
        {
            readonly T[] array;
            readonly Func<T, V> f;

            public ArraySelectorToList(T[] array, Func<T, V> f) =>
                (this.array, this.f) = (array, f);

            public bool IsReadOnly => true;

            public int Count => array.Length;
            public void CopyTo(V[] array, int arrayIndex)
            {
                var idx = 0;
                foreach (var item in this.array)
                    array[arrayIndex + idx++] = f(item);
            }

            public IEnumerator<V> GetEnumerator() => Impl.GetEnumerator(array, f);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(V item) => throw new System.NotImplementedException();
            public void Clear() => throw new System.NotImplementedException();
            public bool Contains(V item) => throw new System.NotImplementedException();
            public bool Remove(V item) => throw new System.NotImplementedException();
        }

        internal sealed class ToListImpl<T> : Consumer<T, List<T>>
        {
            public ToListImpl() : base (new List<T>()) {}

            public override ProcessNextResult ProcessNext(T input)
            {
                Result.Add(input);
                return ProcessNextResult.OK;
            }
        }

        internal static List<TSource> ToList<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            switch(source)
            {
                case Consumable<TSource> c: return c.ToList();

                default: return new List<TSource>(source);
            }
        }
    }
}
