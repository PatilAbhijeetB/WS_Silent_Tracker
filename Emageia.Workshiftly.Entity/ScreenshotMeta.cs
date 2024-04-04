using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class ScreenshotMeta
    {

        public int id { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string key { get; set; }
        public string objectType { get; set; }
        public string action { get; set; }
        public string contentEncoding { get; set; }
        public string contentType { get; set; }
        public string url { get; set; }
        public int expiration { get; set; }
        public string data { get; set; }
        public bool completed { get; set; }

    }
}
