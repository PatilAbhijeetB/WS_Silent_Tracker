using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class Screenshot
    {
        [Column("RowId")]
        public int rowId { get; set; }

        [Column("UserId", TypeName = "VARCHAR")]
        public string userId { get; set; }

        [Column("CampanyId", TypeName = "VARCHAR")]
        public string companyId { get; set; }


        [Column("TimeStamp")]
        public Int32 timestamp { get; set; }


        [Column("Data", TypeName = "VARCHAR")]
        public string Data { get; set; }


        [Column("MineType", TypeName = "VARCHAR")]
        public string mimeType { get; set; }


        [Column("FileName", TypeName = "VARCHAR")]
        public string fileName { get; set; }


        [Column("IsSynced", TypeName = "VARCHAR")]
        public bool synced { get; set; }
        
    }
}
