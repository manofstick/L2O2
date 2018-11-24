using System;
using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class ConsumableEnumerator<T> : Consumer<T, T>, IEnumerator<T>
    {
        protected ConsumableEnumerator() : base(default(T)) { }

        internal virtual Chain<T> StartOfChain { get; }

        protected T result;

        public override bool ProcessNext(T input, ref T result)
        {
            result = input;
            return true;
        }

        public virtual T Current => result;
        object IEnumerator.Current => result;
        public virtual void Dispose() => StartOfChain.ChainDispose();
        public virtual void Reset() => throw new NotSupportedException();

        public abstract bool MoveNext();
    }
}
