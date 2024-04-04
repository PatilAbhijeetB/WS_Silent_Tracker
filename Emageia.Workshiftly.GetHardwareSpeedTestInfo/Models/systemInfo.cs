using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    public class systemInfo
    {
        public Int32 dataCreatedTime { get; set; }
        public Int32 deviceId { get; set; }
        public string machineName { get; set; }
        public string machineFullName { get; set; }
        public string processor { get; set; }
        public string processorId { get; set; }
        public string ram { get; set; }
        public string osName { get; set; }
        public string osArchitecture { get; set; }
        public string osVersion { get; set; }
        public string windowsVersion { get; set; }
        public string osType { get; set; }
        public string windowsReleaseId { get; set; }
        public string windowsDisplayVersion { get; set; }
        public string windowsUpdateBuildRevision { get; set; }
      
    }
}
