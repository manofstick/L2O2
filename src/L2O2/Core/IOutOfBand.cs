using System;

namespace L2O2.Core
{
    interface IOutOfBand
    {
        void StopFurtherProcessing();
        void ListenForStopFurtherProcessing(Action a);
    }
}
