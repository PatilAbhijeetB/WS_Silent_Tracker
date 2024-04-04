using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    public class geoLocation
    {
        public Int32 dataCreatedTime { get; set; }
        public Int32 deviceId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string timezone { get; set; }
       
    }
}
