using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class ActivietyWindows
    {
        public  int rowId { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string processId { get; set; }

        [Column(TypeName = "VARCHAR")]
        public  string appName { get; set; }

        [Column(TypeName = "VARCHAR")]
        public  string title { get; set; }

        public  Int32 startedTimestamp { get; set; }
       
        public  Int32 endTimestamp { get; set; }

        
        public Int32 focusDuration { get; set; }

        [Column(TypeName = "VARCHAR")]

        public  bool isSynced { get; set; }

        public int deviceId { get; set; }

        [Column(TypeName = "VARCHAR")]
        public  string userId { get; set; }

        [Column(TypeName = "VARCHAR")]
        public  string companyId { get; set; }

       
        [Column(TypeName = "VARCHAR")]
        public  string isPartial
        { get; set; 
        }
        [Column(TypeName = "VARCHAR")]
        public  string operatingSystem { get; set; }
        
    }
}
