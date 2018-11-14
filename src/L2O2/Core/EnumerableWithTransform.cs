﻿using System;

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

        internal SeqConsumer<U, TResult> CreatePipeline<TResult>(Func<SeqConsumer<U, TResult>> getConsumer, out SeqConsumerActivity<T, U> activity)
        {
            var seqConsumer = getConsumer();
            activity = CreateActivityPipeline(seqConsumer);
            return seqConsumer;
        }

        internal ISeqTransform<T,V> ComposeWith<V>(ISeqTransform<U, V> next)
        {
            return new CompositionTransform<T, U, V>(transform, next);
        }
    }
}