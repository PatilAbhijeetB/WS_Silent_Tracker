using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity.ServiceModels
{
    public class BandwidthDetails
    {
        public string machineName { get; set; }
        public string hostName { get; set; }
        public string hostIP { get; set; }
        public string Domain { get; set; }
        public string Host { get; set; }
        public string IP { get; set; }
        public string Jitter { get; set; }
        public string ISPName { get; set; }
        public string WANIP { get; set; }
        public string Ping { get; set; }
        public string downloadSpeed { get; set; }
        public string uploadSpeed { get; set; }
        public bool isServer { get; set; }

    }
}
