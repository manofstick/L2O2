﻿using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    static class Utils
    {
        internal static Consumable<U> CreateConsumable<T, U>(IEnumerable<T> e, ITransmutation<T, U> transform)
        {
            switch (e)
            {
                case T[] array:
                    return new ConsumableArray<T, T, U>(array, IdentityTransform<T>.Instance, transform);

                case List<T> list:
                    return new ConsumableList<T, T, U>(list, IdentityTransform<T>.Instance, transform);

                default:
                    return new ConsumableEnumerable<T, T, U>(e, IdentityTransform<T>.Instance, transform);
            }
        }

        internal static Consumable<U> PushTransform<T, U>(IEnumerable<T> e, ITransmutation<T, U> transform)
        {
            switch (e)
            {
                case Consumable<T> consumable:
                    return consumable.AddTail(transform);

                default:
                    return CreateConsumable(e, transform);
            }
        }

        internal static Result Consume<T, Result>(IEnumerable<T> e, Consumer<T, Result> consumer)
        {
            switch (e)
            {
                case Consumable<T> consumable:
                    return consumable.Consume(consumer);

                default:
                    return CreateConsumable(e, IdentityTransform<T>.Instance).Consume(consumer);
            }
        }

        internal class EmptyEnumerator<T> 
            : IEnumerator<T>
        {
            private EmptyEnumerator() { }

            public bool MoveNext() { return false; }
            public void Reset() {}
            public void Dispose() { }
            public T Current => default(T);
            object IEnumerator.Current => null;

            public static IEnumerator<T> Instance { get; } = new EmptyEnumerator<T>();
        }
    }
}
