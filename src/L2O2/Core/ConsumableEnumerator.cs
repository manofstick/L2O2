using System;
using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class ConsumableEnumerator<T> : Consumer<T, T>, IEnumerator<T>
    {
        protected ConsumableEnumerator() : base(default(T)) { }

        internal virtual Chain<T> StartOfChain { get; }

        protected Status<T> result;

        public override bool ProcessNext(T input, ref Status<T> result)
        {
            result.Value = input;
            return true;
        }

        public virtual T Current => result.Value;
        object IEnumerator.Current => result.Value;
        public virtual void Dispose() => StartOfChain.ChainDispose();
        public virtual void Reset() => throw new NotSupportedException();

        public abstract bool MoveNext();
    }
}
