using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity.ServiceModels
{
    public class SystemCaptureDetails
    {
     
        public string machineName { get; set; }
        public string machineFullName { get; set; }
        public string processor { get; set; }
        public string processorId { get; set; }
        public string ram { get; set; }
        public string productId { get; set; }
        public string osName { get; set; }
        public string osArchitecture { get; set; }
        public string osVersion { get; set; }
        public string windowsVersion { get; set; }
        public string oStype { get; set; }
        public bool isWorkstation { get; set; }
        public bool isServer { get; set; }
        public string windowsReleaseId { get; set; }
        public string windowsDisplayVersion { get; set; }
        public string windowsUpdateBuildRevision { get; set; }
        public float pageFile { get; set; }
        public string memoryCommite { get; set; }
        public string memoryPoolPaged { get; set; }
        public string memoryPoolNonPaged { get; set; }
        public string memoryCachedBytes { get; set; }
        public string memoryAvailable { get; set; }

    }
}
