using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Permissions;

namespace ArgusTV.Common.StaThreadSyncronizer
{
    [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
    public class StaSynchronizationContext : SynchronizationContext, IDisposable
    {
        private BlockingQueue<SendOrPostCallbackItem> _queue;
        private StaThread _staThread;

        public StaSynchronizationContext(string threadName)
        {
            _queue = new BlockingQueue<SendOrPostCallbackItem>();
            _staThread = new StaThread(_queue, threadName);
            _staThread.Start();
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            // to avoid deadlock!
            if (Thread.CurrentThread.ManagedThreadId == _staThread.ManagedThreadId)
            {
                d(state);
                return;
            }

            // create and queue an item for execution
            SendOrPostCallbackItem item = new SendOrPostCallbackItem(d, state, ExecutionType.Send);
            _queue.Enqueue(item);
            // wait for the item execution to end
            item.ExecutionCompleteWaitHandle.WaitOne();

            // if there was an exception, throw it on the caller thread, not the sta thread.
            if (item.ExecutedWithException)
            {
                throw item.Exception;
            }
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            // queue the item and don't wait for its execution. This is risky because
            // an unhandled exception will terminate the STA thread. Use with caution.
            SendOrPostCallbackItem item = new SendOrPostCallbackItem(d, state, ExecutionType.Post);
            _queue.Enqueue(item);
        }

        public void Dispose()
        {
            _staThread.Stop();
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }
}
