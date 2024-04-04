using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity.HttpClientModel.activewindo
{
    public class ActiveWindowReturn
    {
        public bool error { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public AcriveWindowJobId data { get; set; }
    }

    public class AcriveWindowJobId
    {
        public string jobId { get; set; }
    }
}
