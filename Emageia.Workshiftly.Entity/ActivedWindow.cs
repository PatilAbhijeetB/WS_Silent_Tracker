using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class ActivedWindow
    { 
        public static int Id { get; set; }
        public static string ProcessId { get; set; }
        public static string AppName { get; set; }
        public static string Title { get; set; }
        public static Int32 StartedTimestamp { get; set; }
        public static Int32 EndTimestamp { get; set; }
        public static Int32 FocusDuration { get; set; }
        public static bool isWebBrowser { get; set; }
        public static bool IsSynced { get; set; }
        public static int deviceId { get; set; }
        public static string UserId { get; set; }
        public static string CompanyId { get; set; }
        public static string IsPartial { get; set; }
        public static string OperatingSystem { get; set; }
    }
}