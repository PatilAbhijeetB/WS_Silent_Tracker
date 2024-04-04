using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class CompanyConfiguration
    {
        public string id { get; set; }
        public int appIdleTime { get; set; }
        public bool activeSilentTracking { get; set; }
        public bool isScreenCapturing { get; set; }
        public int screenShotsPerHour { get; set; }
        public int maximumDevicesAllowedPerUser { get; set; }
    //    public int numberOfScreenshotsPerHour { get; set; }
        public bool isTrackWithinShift { get; set; }
        public bool allowUsersToAddBreakReasons { get; set; }
        public List<BreakReasons> breakReasons { get; set; }
       
        public bool allowNotification { get; set; }
        public bool isTwoFactorAuthenticationEnabled { get; set; }
        public bool isAllowReportExportForOtherUsers { get; set; }
        public string allowedReportExportFormat { get; set; }
        public Int32 createdAt { get; set; }
        public Int32 updatedAt { get; set; }

    }
}
