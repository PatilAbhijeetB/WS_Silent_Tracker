using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity.HttpClientModel
{
    public class ScreenshotCreatedPresigndUrlObject
    {
        public string action { get; set; }
        public string key { get; set; }
        public int expiration { get; set; }
        public string url { get; set; }
    }
}
