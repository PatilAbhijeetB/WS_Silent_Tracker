using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity.HttpClientModel
{
    public class ServerReturnObject
    {
        public bool error { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public List<ScreenshotMeta>  data { get; set; }
       // public List<ScreenshotCreatedPresigndUrlObject>  data { get; set; }
    }
}
