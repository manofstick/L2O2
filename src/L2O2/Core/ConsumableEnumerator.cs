using System;
using System.Collections;
using System.Collections.Generic;

namespace L2O2.Core
{
    internal abstract class ConsumableEnumerator<T> : Consumer<T, T>, IEnumerator<T>
    {
        protected ConsumableEnumerator() : base(default(T)) { }

        internal virtual ConsumerActivity Activity { get; set; }

        public override bool ProcessNext(T input)
        {
            Result = input;
            return true;
        }

        public virtual T Current => Result;
        object IEnumerator.Current => Result;
        public virtual void Dispose() => Activity.ChainDispose();
        public virtual void Reset() => throw new NotSupportedException();

        public abstract bool MoveNext();
    }
}
