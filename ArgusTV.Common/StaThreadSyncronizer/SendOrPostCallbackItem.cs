using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArgusTV.Common.StaThreadSyncronizer
{
    internal enum ExecutionType
    {
        Post,
        Send
    }

    internal class SendOrPostCallbackItem
    {
        private object _state;
        private ExecutionType _executionType;
        private SendOrPostCallback _method;
        private ManualResetEvent _asyncWaitHandle = new ManualResetEvent(false);
        private Exception _exception = null;

        internal SendOrPostCallbackItem(SendOrPostCallback callback, object state, ExecutionType type)
        {
            _method = callback;
            _state = state;
            _executionType = type;
        }

        internal Exception Exception
        {
            get { return _exception; }
        }

        internal bool ExecutedWithException
        {
            get { return _exception != null; }
        }

        // this code must run ont the STA thread
        internal void Execute()
        {
            if (_executionType == ExecutionType.Send)
            {
                Send();
            }
            else
            {
                Post();
            }
        }

        // calling thread will block until mAsyncWaitHanel is set
        internal void Send()
        {
            try
            {
                // call the thread
                _method(_state);
            }
            catch (Exception e)
            {
                _exception = e;
            }
            finally
            {
                _asyncWaitHandle.Set();
            }
        }

        /// <summary>
        /// Unhandle execptions will terminate the STA thread
        /// </summary>
        internal void Post()
        {
            _method(_state);
        }

        internal WaitHandle ExecutionCompleteWaitHandle
        {
            get { return _asyncWaitHandle; }
        }
    }
}
