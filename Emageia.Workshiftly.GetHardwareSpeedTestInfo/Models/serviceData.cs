using Emageia.Workshiftly.Entity.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    public class serviceData
    {
        public Int32 dataCreatedTime { get; set; }
        public Int32 deviceId { get; set; }
        public List<ServiceDeatails> services { get; set; }
       
    }
}
