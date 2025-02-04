﻿
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails.Environment.Mocks
{
    public class EnvironmentProviderMock : IEnvironment
    {
        private bool _is64BitOperatingSystem;

        public EnvironmentProviderMock(bool is64BitOperatingSystem)
        {
            this._is64BitOperatingSystem = is64BitOperatingSystem;
        }
        public bool Is64BitOperatingSystem()
        {
            return this._is64BitOperatingSystem;
        }
    }
}
