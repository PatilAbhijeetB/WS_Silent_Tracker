using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity.ServiceModels
{
    public class UtilizationDetails
    {
        public float cpuUtilization { get; set; }
        public float ramUtilization { get; set; }
        public float pageUtilization { get; set; }
        public float networkUtilization { get; set; }
    }
}
