using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{

    /// <summary>
    ///  client loging -> return => device
    /// </summary>
    public class DeviceConfiguration
    {
        public int id { get; set; }
        public string name { get; set; }
        public DeviceSetting loggedInDevice { get; set; }
        public string profileImage { get; set; } = "";

    }
}
