﻿namespace L2O2.Core
{
    interface ISeqTransform<T,U>
    {
        SeqConsumerActivity<T, V> Compose<V>(ISeqConsumer consumer, SeqConsumerActivity<U,V> activity);
    }
}
