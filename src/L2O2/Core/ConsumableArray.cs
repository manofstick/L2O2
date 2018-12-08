using System;
using System.Collections;
using System.Collections.Generic;
using static L2O2.Consumable;

namespace L2O2.Core
{
    internal class ConsumableArray<T, U, V> : ConsumableWithComposition<T, U, V>
    {
        private readonly T[] array;

        public ConsumableArray(T[] array, ITransmutation<T, U> first, ITransmutation<U, V> second) : base(first, second) =>
            this.array = array;

        public override Consumable<W> Create<VV, W>(ITransmutation<T, VV> first, ITransmutation<VV, W> second) =>
            new ConsumableArray<T, VV, W>(array, first, second);

        public override IEnumerator<V> GetEnumerator() =>
            Impl.GetEnumerator(array, this);

        public override Result Consume<Result>(Consumer<V, Result> consumer) =>
            Impl.Consume(array, this, consumer);

        public override List<V> ToList()
        {
            if (first == IdentityTransform<T>.Instance)
            {
                switch(second)
                {
                    case SelectImpl<T, V> select: return new List<V>(new ArraySelectorToList<T,V>(array, select.Selector));
                }
            }
            return base.ToList();
        }
    }
}
