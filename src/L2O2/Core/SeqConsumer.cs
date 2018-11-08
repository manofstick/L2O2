using System;

namespace L2O2.Core
{
    abstract class SeqConsumer<T, R>
        : SeqConsumerActivity<T, T>
    {
        private Action<PipeIdx> listeners;

        protected SeqConsumer(R initalResult)
        {
            Result = initalResult;
        }

        public R Result { get; protected set; }

        public override void ChainComplete(PipeIdx pipeIdx) {}
        public override void ChainDispose() {}

        //member __.HaltedIdx = haltedIdx

        //interface ISeqConsumer with
        //    member this.StopFurtherProcessing pipeIdx =
        //        let currentIdx = haltedIdx
        //        haltedIdx<- pipeIdx
        //        if currentIdx = 0 && haltedIdx<> 0 then
        //            match listeners with
        //            | null -> ()
        //            | a -> a.Invoke pipeIdx

        //    member this.ListenForStopFurtherProcessing action =
        //        listeners <- Delegate.Combine (listeners, action) :?> Action<PipeIdx>
    }
}
