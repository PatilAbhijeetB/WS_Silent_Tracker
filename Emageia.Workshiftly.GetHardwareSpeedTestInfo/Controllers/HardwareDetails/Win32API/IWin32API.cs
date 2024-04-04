using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.Win32API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.Environment.Win32API
{
    public interface IWin32API
    {
        NTSTATUS RtlGetVersion(ref OSVERSIONINFOEX versionInfo);
        int GetSystemMetrics(SystemMetric smIndex);
    }
}
