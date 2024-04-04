using Emageia.Workshiftly.Entity.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    public class bandwidthData
    {
        
        public Int32 dataCreatedTime { get; set; }
        public Int32 deviceId { get; set; }
        public string ispName { get; set; }
        public string downloadSpeed { get; set; }
        public string uploadSpeed { get; set; }
        public string jitter { get; set; }
        public string ping { get; set; }

        
    }
}
