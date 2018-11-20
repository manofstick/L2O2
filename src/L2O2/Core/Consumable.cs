﻿using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class Consumable<T>
        : IEnumerable<T> 
    {
        public abstract Result Consume<Result>(SeqConsumer<T,Result> consumer);
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public abstract ISeqTransform<T> TailTransform { get; }
        public abstract Consumable<V> ReplaceTail<U, V>(ISeqTransform<U, V> selectImpl);
        public abstract Consumable<U> Transform<U>(ISeqTransform<T, U> transform);
    }
}
