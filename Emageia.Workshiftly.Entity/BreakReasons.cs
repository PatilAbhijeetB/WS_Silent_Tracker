using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class BreakReasons
    {
        public string title { get; set; }
        public bool isStartedAutomatically { get; set; }
        public Int32 duration { get; set; }
    }
}
