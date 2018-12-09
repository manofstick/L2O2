using System;
using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class ConsumableEnumerator<T> : Consumer<T, T>, IEnumerator<T>
    {
        protected ProcessNextResult processNextResult = ProcessNextResult.Flow;

        protected ConsumableEnumerator() : base(default(T)) { }

        internal virtual Chain StartOfChain { get; }

        public override ProcessNextResult ProcessNext(T input)
        {
            Result = input;
            return Flow;
        }

        public virtual T Current => Result;
        object IEnumerator.Current => Result;
        public virtual void Dispose() => StartOfChain.ChainDispose();
        public virtual void Reset() => throw new NotSupportedException();

        public abstract bool MoveNext();
    }
}
