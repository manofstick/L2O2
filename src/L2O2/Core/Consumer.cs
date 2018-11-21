using System;

namespace L2O2.Core
{
    abstract class Consumer<T, R>
        : ConsumerActivity<T, T>
        , IOutOfBand
    {
        private Action listeners;

        protected Consumer(R initalResult)
        {
            Result = initalResult;
        }

        public R Result { get; protected set; }
        public bool Halted { get; protected set; }

        public override void ChainComplete() { }
        public override void ChainDispose() { }

        public void ListenForStopFurtherProcessing(Action a)
        {
            throw new NotImplementedException();
        }

        public void StopFurtherProcessing()
        {
            Halted = true;
        }

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
