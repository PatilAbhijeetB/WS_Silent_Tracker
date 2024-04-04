using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    public class monitorInfo
    {
        public Int32 dataCreatedTime { get; set; }
        public Int32 deviceId { get; set; }
        public string monitorName{ get; set; }
        public string caption { get; set; }
        public string installDate { get; set; }
        public string creationClassName { get; set; }
        public string hardwareDeviceId { get; set; }
        public string monitorManufacture { get; set; } 
        public string pnpDeviceId { get; set; }
        public string monitorType { get; set; }
        public string pixelsPerXLogicalInch { get; set; }
        public string pixelsPerYLogicalInch { get; set; }
        public string status { get; set; }
        public string systemCreationClassName { get; set; }
        public string systemName { get; set; }
    }
}
