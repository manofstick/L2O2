using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        internal sealed class ToListImpl<T> : Consumer<T, List<T>>
        {
            public ToListImpl() : base (new List<T>()) {}

            public override ProcessNextResult ProcessNext(T input)
            {
                Result.Add(input);
                return ProcessNextResult.Flow;
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
