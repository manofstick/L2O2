using System;

namespace L2O2.Core
{
    [Flags]
    enum ProcessNextResult
    {
        OK             = 0x00,

        Filtered       = 0x02,

        Halted         = 0x80,
        HaltedActivity = 0x81,
        HaltedConsumer = 0x82,
    }

    static class ProcessNextResultHelper
    {
        public static bool IsHalted(this ProcessNextResult result) =>
            (result & ProcessNextResult.Halted) == ProcessNextResult.Halted;

        public static bool IsOK(this ProcessNextResult result) =>
            result == ProcessNextResult.OK;
    }
}
