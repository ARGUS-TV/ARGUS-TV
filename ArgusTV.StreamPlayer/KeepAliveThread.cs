using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ArgusTV.Common.Threading;
using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.StreamPlayer
{
    internal class KeepAliveThread : WorkerThread
    {
        private static int _checkInterval = 20 * 1000;

        public KeepAliveThread()
            : base("Keep-alive")
        {
        }

        public LiveStream LiveStream { get; set; }

        protected override void Run()
        {
            var controlProxy = new ControlServiceProxy();

            bool exited = false;
            while (!exited)
            {
                if (!controlProxy.KeepLiveStreamAlive(this.LiveStream))
                {
                    break;
                }
                exited = this.StopThreadEvent.WaitOne(_checkInterval, false);
            }
        }
    }
}
