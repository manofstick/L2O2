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
                    case WhereImpl<T> where: return (IEnumerator<V>)GetEnumerator(array, where.Predicate);
                }
            }

            if (array.Rank == 1 && array.GetLowerBound(0) == 0 && array.GetUpperBound(0) < int.MaxValue)
                 return ConsumableArrayEnumerator<T, V>.Create(array, composition.Composed);

            return NonStandardArray(array, composition.Composed);
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
                    case WhereImpl<T> where: return (IEnumerator<V>)GetEnumerator(lst, where.Predicate);
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
                    case WhereImpl<T> where: return (IEnumerator<V>)GetEnumerator(e, where.Predicate);
                }
            }

            return ConsumableEnumerableEnumerator<T, V>.Create(e, composition.Composed);
        }

        class SetResultConsumer<T> : Consumer<T, T>
        {
            public SetResultConsumer() : base(default(T)) { }

            public override ProcessNextResult ProcessNext(T input)
            {
                Result = input;
                return OK;
            }
        }

        private static IEnumerator<V> NonStandardArray<T, V>(T[] array, ITransmutation<T, V> composed)
        {
            var consumer = new SetResultConsumer<V>();
            var activity = composed.Compose(consumer);
            foreach (var item in array)
            {
                var rc = activity.ProcessNext(item);
                if (rc.IsOK())
                    yield return consumer.Result;
                else if (rc.IsHalted())
                    break;
            }
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


        private static IEnumerator<T> GetEnumerator<T>(T[] array, Func<T, bool> predicate)
        {
            foreach (var item in array)
                if (predicate(item))
                    yield return item;
        }


        private static IEnumerator<T> GetEnumerator<T>(List<T> lst, Func<T, bool> predicate)
        {
            foreach (var item in lst)
                if (predicate(item))
                    yield return item;
        }

        private static IEnumerator<T> GetEnumerator<T>(IEnumerable<T> e, Func<T, bool> predicate)
        {
            foreach (var item in e)
                if (predicate(item))
                    yield return item;
        }
    }
}
