using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.MajorVersion10
{
    public class VersionInfo
    {
        public Version Version { get; private set; }

        public VersionInfo(int major, int minor, int build)
        {
            this.Version = new Version(major, minor, build);
        }
    }

}
