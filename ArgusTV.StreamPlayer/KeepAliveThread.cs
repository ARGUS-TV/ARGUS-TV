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
            bool exited = false;
            while (!exited)
            {
                if (!Proxies.ControlService.KeepLiveStreamAlive(this.LiveStream).Result)
                {
                    break;
                }
                exited = this.StopThreadEvent.WaitOne(_checkInterval, false);
            }
        }
    }
}
