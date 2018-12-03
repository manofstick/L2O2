using System;

namespace L2O2.Core
{
    abstract class Chain
    {
        public abstract void ChainComplete();
        public abstract void ChainDispose();
    }

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

    abstract class Chain<T> : Chain
    {
        public abstract ProcessNextResult ProcessNext(T input);
    }

    abstract class Chain<T, U> : Chain<T>
    {
    }

    abstract class Activity<T, U, V> : Chain<T,V>
    {
        protected readonly Chain<U, V> next;

        protected Activity(Chain<U, V> next)
        {
            this.next = next;
        }

        public override void ChainComplete() => next.ChainComplete();
        public override void ChainDispose() => next.ChainDispose();
    }

    sealed class ChainEnd { private ChainEnd() { } }

    abstract class Consumer<T, R>
        : Chain<T, ChainEnd>
    {
        protected Consumer(R initalResult)
        {
            Result = initalResult;
        }

        public R Result { get; protected set; }

        public override void ChainComplete() { }
        public override void ChainDispose() { }
    }
}
