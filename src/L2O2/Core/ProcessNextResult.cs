using System;

namespace L2O2.Core
{
    [Flags]
    enum ProcessNextResult
    {
        Filter = 0x00,
        Flow   = 0x01,
        Stop   = 0x02,
        Consumer = 0x04,

        StoppedConsumer = Stop | Consumer,
    }

    static class ProcessNextResultHelper
    {
        public static bool IsStopped(this ProcessNextResult result) =>
            (result & ProcessNextResult.Stop) == ProcessNextResult.Stop;

        public static bool IsFlowing(this ProcessNextResult result) =>
            (result & ProcessNextResult.Flow) == ProcessNextResult.Flow;

        public static bool IsStoppedConsumer(this ProcessNextResult result) =>
            (result & ProcessNextResult.StoppedConsumer) == ProcessNextResult.StoppedConsumer;
    }
}
