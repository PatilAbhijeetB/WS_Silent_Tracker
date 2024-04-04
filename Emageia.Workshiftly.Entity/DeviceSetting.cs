using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.Entity
{
    public class DeviceSetting
    {
        public int id { get; set; }
        /// <summary>
        /// Loggin Device Auto gen Id
        /// </summary>
        public int deviceID { get; set; }

        /// <summary>
        /// Loggin User Id
        /// </summary>

        [Column(TypeName ="VARCHAR")]
        public String userId { get; set; }

        
        /// <summary>
        /// User Logging from divece or not
        /// </summary>
        public int isLogging { get; set; }


        /// <summary>
        /// Logging device Mac Address
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public string macAddress { get; set; }


        /// <summary>
        /// Logging device IpAddress
        /// </summary>

        [Column(TypeName = "VARCHAR")]
        public String ipAddress { get; set; }


        /// <summary>
        /// Loging Device Machine Name
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public string machineName { get; set; }


        /// <summary>
        /// Logging Device Machine User Name
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public string machineUserName { get; set; }


        /// <summary>
        /// Logging Device Platfrom
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public String platform { get; set; }


        /// <summary>
        /// Logging Device version
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public string version { get; set; }


        /// <summary>
        /// Logging Device Os Name
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public string osName { get; set; }


        /// <summary>
        /// Logging Device OsVersionn Major
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public String osVersionMajor { get; set; }


        /// <summary>
        /// Logging Device Os Version Minor
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public string osVersionMinor { get; set; }


        /// <summary>
        /// Logging device Create Time
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public Int32 createdAt { get; set; }

        /// <summary>
        /// Logging Device Update record time
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        public Int32 updatedAt { get; set; }


        public UserDevice userDevice { get; set; }
    }
}
