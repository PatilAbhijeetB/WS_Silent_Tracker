using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity.HttpClientModel.DownloadFile
{
    public class ServerReturnUrlObject
    {
        public bool error { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public string version { get; set; }
    }
}
