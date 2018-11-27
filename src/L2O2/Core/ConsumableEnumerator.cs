using System;
using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class ConsumableEnumerator<T> : Consumer<T, T>, IEnumerator<T>
    {
        protected Status<T> result;

        protected ConsumableEnumerator() : base(default(T)) { }

        public override bool ProcessNext(T input, ref Status<T> result)
        {
            result.Value = input;
            return true;
        }

        public abstract bool MoveNext();

        public virtual T Current => result.Value;
        object IEnumerator.Current => result.Value;

        public virtual void Reset() => throw new NotSupportedException();

        protected abstract Chain<T> StartOfChain { get; }
        public virtual void Dispose() => StartOfChain.ChainDispose();
    }
}
