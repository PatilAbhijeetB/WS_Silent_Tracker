using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.DataModels
{
    public class LogAppData
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string LogType { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string LogData { get; set; }
    }
}
