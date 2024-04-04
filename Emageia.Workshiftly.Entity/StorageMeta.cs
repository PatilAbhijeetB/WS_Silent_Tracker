using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class StorageMeta
    {
        public int Id { get; set; }
        public string ObjectType { get; set; }
        public string Key { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public string Expiration { get; set; }
        public string ExpirationTimestamp { get; set; }
        public string Url { get; set; }
        public string Competed { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
      
    }
}
