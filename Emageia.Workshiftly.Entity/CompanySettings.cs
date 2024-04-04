using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class CompanySettings
    {
        public string id { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string companyId { get; set; }
        public string authToken { get; set; }
        public bool isActive { get; set; }
        public bool isClientActive { get; set; }
        public bool isAllowOfflineTask { get; set; }
       
    }
}
