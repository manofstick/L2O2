﻿namespace L2O2.Core
{
    class IdentityTransform<T> : ISeqTransform<T,T>
    {
        public static ISeqTransform<T, T> Instance { get; } = new IdentityTransform<T>();

        public SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<T, V> next)
        {
            return next;
        }
    }
}
