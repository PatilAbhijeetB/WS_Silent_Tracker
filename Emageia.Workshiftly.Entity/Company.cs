using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class Company
    {
        public string id { get; set; }
        public string name { get; set; }
        public string timezone { get; set; }
        public string timeFormat { get; set; }
        public string ownerId { get; set; }
        public string email { get; set; }
        public string configurationId { get; set; }
        public Int32 startShift { get; set; }
        public Int32 endShift { get; set; }
        public Int32 createdAt { get; set; }
        public Int32 updatedAt { get; set; }
    }
}
