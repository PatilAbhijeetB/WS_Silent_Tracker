using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class SyncUpdateStatus
    {
        public int Id { get; set; }
        public bool UpdatingWpf { get; set; } // true download file
        public bool UpdatingServer { get; set; } // true extractting file
        public bool SuccessfulyDownload { get; set; }
        public bool Extract { get; set; }
        public string downloadpath { get; set; }
        public string storedPath { get; set; }
    }
}
