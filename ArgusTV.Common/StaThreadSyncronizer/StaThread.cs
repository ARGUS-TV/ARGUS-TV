using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArgusTV.Common.StaThreadSyncronizer
{
    internal class StaThread
    {
        private Thread _staThread;
        private IQueueReader<SendOrPostCallbackItem> _consumerQueue;
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);

        internal StaThread(IQueueReader<SendOrPostCallbackItem> reader, string threadName)
        {
            _consumerQueue = reader;
            _staThread = new Thread(Run);
            _staThread.Name = threadName;
            _staThread.SetApartmentState(ApartmentState.STA);
        }

        internal int ManagedThreadId { get; private set; }

        internal void Start()
        {
            _staThread.Start();
        }

        internal void Join()
        {
            _staThread.Join();
        }

        private void Run()
        {
            this.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            while (true)
            {
                bool stop = _stopEvent.WaitOne(0);
                if (stop)
                {
                    break;
                }

                SendOrPostCallbackItem workItem = _consumerQueue.Dequeue();
                if (workItem != null)
                    workItem.Execute();
            }
        }

        internal void Stop()
        {
            _stopEvent.Set();
            _consumerQueue.ReleaseReader();
            _staThread.Join();
            _consumerQueue.Dispose();
        }
    }
}
