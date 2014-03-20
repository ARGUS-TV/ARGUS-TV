using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArgusTV.Common.StaThreadSyncronizer
{
    internal interface IQueueReader<T> : IDisposable
    {
        T Dequeue();
        void ReleaseReader();
    }

    internal interface IQueueWriter<T> : IDisposable
    {
        void Enqueue(T data);
    }

    internal class BlockingQueue<T> : IQueueReader<T>, IQueueWriter<T>, IDisposable
    {
        private Queue<T> _queue = new Queue<T>();
        private Semaphore _semaphore = new Semaphore(0, int.MaxValue);
        private ManualResetEvent _killThreadEvent = new ManualResetEvent(false);
        private WaitHandle[] _waitHandles;

        public BlockingQueue()
        {
            _waitHandles = new WaitHandle[2] { _semaphore, _killThreadEvent };
        }

        public void Enqueue(T data)
        {
            lock (_queue) _queue.Enqueue(data);
            _semaphore.Release();
        }

        public T Dequeue()
        {
            WaitHandle.WaitAny(_waitHandles);
            lock (_queue)
            {
                if (_queue.Count > 0)
                {
                    return _queue.Dequeue();
                }
            }
            return default(T);
        }

        public void ReleaseReader()
        {
            _killThreadEvent.Set();
        }

        void IDisposable.Dispose()
        {
            if (_semaphore != null)
            {
                _semaphore.Close();
                _queue.Clear();
                _semaphore = null;
            }
        }
    }
}
