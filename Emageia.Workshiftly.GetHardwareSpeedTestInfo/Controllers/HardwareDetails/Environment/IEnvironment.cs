using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.Environment
{
    public interface IEnvironment
    {
        /// <summary>
        /// Determines whether the current operating system is a 64-bit operating system.
        /// </summary>
        /// <returns></returns>
        bool Is64BitOperatingSystem();
    }
}
