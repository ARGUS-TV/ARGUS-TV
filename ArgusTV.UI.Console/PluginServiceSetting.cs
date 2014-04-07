using System;
using System.Collections.Generic;
using System.Text;

namespace ArgusTV.UI.Console
{
    internal class PluginServiceSetting
    {
        public PluginServiceSetting(string name, string fullName, int tcpPort)
        {
            this.Name = name;
            this.FullName = fullName;
            this.TcpPort = tcpPort;
        }

        public string Name { get; set; }

        public string FullName { get; set; }

        public int TcpPort { get; set; }
    }
}
