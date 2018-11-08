using System;

namespace L2O2.Core
{
    interface ISeqConsumer
    {
        void StopFurtherProcessing(PipeIdx pipeIdx);
        void ListenForStopFurtherProcessing(Action<PipeIdx> a);
    }
}
