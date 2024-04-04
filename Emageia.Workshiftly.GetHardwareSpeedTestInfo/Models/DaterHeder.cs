using Emageia.Workshiftly.Entity.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    public class DaterHeder
    {
        public string userId { get; set; }
        public Int32 dataCreatedTime { get; set; }
        public Int32 data_Created_Time { get; set; }
        public Int32 deviceId { get; set; }
        public Int32 device_Id { get; set; }
        public string isp_name { get; set; }
        public string download_speed { get; set; }
        public string upload_speed { get; set; }
        public string jitter { get; set; }
        public string ping { get; set; }

        public BandwidthDetails network { get; set; }
        public LocationCoordinates location { get; set; }
        public SystemCaptureDetails systemDetails { get; set; }
        public UtilizationDetails utilization { get; set; }
        public List<SoftwareDetails> software { get; set; }
        public List<ServiceDeatails> services { get; set; }

    }
}
