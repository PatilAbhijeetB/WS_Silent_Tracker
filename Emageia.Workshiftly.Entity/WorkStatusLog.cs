using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class WorkStatusLog
    {
        public int rowId { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string userId { get; set; }

        public string userDate { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string companyId { get; set; }

        public int deviceId { get; set; }
        public Int32 date { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string workStatus { get; set; }

        public Int32 actionTimestamp { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string breakReasonId { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string breakReasonTitle { get; set; }

        public bool isBreakAutomaticallyStart { get; set; }

        public Int32 breakAutomaticallyStartDuration { get; set; }

        public bool isSynced { get; set; }
       
    }
}
