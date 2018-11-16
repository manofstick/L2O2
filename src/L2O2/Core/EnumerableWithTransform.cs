using System;

namespace L2O2.Core
{
    internal abstract class EnumerableWithTransform<T, U> : EnumerableBase<U>
    {
        internal ISeqTransform<T, U> transform;

        public EnumerableWithTransform(ISeqTransform<T, U> transform)
        {
            this.transform = transform;
        }

        internal SeqConsumerActivity<T, U> CreateActivityPipeline<TResult>(SeqConsumer<U, TResult> consumer)
        {
            return transform.Compose(consumer, consumer);
        }

        internal ISeqTransform<T,V> ComposeWith<V>(ISeqTransform<U, V> next)
        {
            return CompositionTransform<T, U, V>.Combine(transform, next);
        }
    }
}