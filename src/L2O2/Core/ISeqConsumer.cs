using System;

namespace L2O2.Core
{
    interface ISeqConsumer
    {
        void StopFurtherProcessing();
        void ListenForStopFurtherProcessing(Action a);
    }
}
