using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class UserDevice
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int deviceId { get; set; }
        public bool isActive { get; set; }
        public bool isLoggedIn { get; set; }
        public Int32 createdAt { get; set; }
        public Int32 updatedAt { get; set;}

    }
}
