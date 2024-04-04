using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails
{
    public static class HardwareInfo
    {

        public static String GetProcessorId()
        {

            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String Id = String.Empty;
            foreach (ManagementObject mo in moc)
            {

                Id = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return Id;

        }

        public static string getProcessorName()
        {

            string strselectqry = "uL0d0qjzpy3iFElki/2qjQDB/tPYu1Q7vDC14w4HmJUHRBj8uPUh7l3Hk0QtVNsghuVyB9PtekL3R/31AqLdQmHtNGNdW0ctWCY1sKmbTJ0aYB3dQe39PI2ZEx3VBU9P";

            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher(Config.StrDecrypt(strselectqry));
            string sysProcessor = string.Empty;
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                sysProcessor = obj["Name"].ToString();

            }
            return sysProcessor;
        }
        /// <summary>
        /// Retrieving HDD Serial No.
        /// </summary>
        /// <returns></returns>
        public static String GetHDDSerialNo()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string result = "";
            foreach (ManagementObject strt in mcol)
            {
                result += Convert.ToString(strt["VolumeSerialNumber"]);
            }
            return result;
        }

        /// <summary>
        /// Retrieving System MAC Address.
        /// </summary>
        /// <returns></returns>
        public static string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }

        /// <summary>
        /// Retrieving Motherboard Manufacturer.
        /// </summary>
        /// <returns></returns>
        public static string GetBoardMaker()
        {

            string strselectqry = "t8cMIL0assArIKu+AN5DSEHH4/plNX2UtKd6COvj+o5LHk0Mc15NEe6BojIFyjwgTlQsJUeFpicY6TlMbWdNC11SIVTI/BWh572RYPVyACmxsCWvN/9nJrURLZKqIq73";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Config.StrDecrypt(strselectqry));
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }

                catch { }

            }

            return "Board Maker: Unknown";

        }

        public static string GetBoardProductId()
        {
            //SELECT * FROM Win32_BaseBoard
            string strselectqry = "Xu52S3EMR7IRlowuXPmgUYQ1Zj7rJJkZ/T33XeKgviJkETVrOi+IeNN2Y02sNuUJWvIE7CTTJ1OcieFLUIE7+tc3dhuIfhtW4EApeey8dKJXnLuIRPjdmOhFGJSw7Aqd";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Config.StrDecrypt(strselectqry));

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Product").ToString();

                }

                catch { }

            }

            return "Product: Unknown";

        }

        /// <summary>
        /// Retrieving CD-DVD Drive Path.
        /// </summary>
        /// <returns></returns>
        public static string GetCdRomDrive()
        {

            string strselectqry = "7bCX+A4kSYEFary5NW0VfwLuYW1dhcZoSWHN/pgzYi6OM8Ki9WiEYowo8Itnyi1KXyArUyrYqbbdLOnokFd4k4/ib+/afiCUatnZJudjMe43p06iqF4KKEQQ3tg5UvL6";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Config.StrDecrypt(strselectqry));
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Drive").ToString();

                }

                catch { }

            }

            return "CD ROM Drive Letter: Unknown";

        }

        /// <summary>
        /// Retrieving BIOS Maker.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSmaker()
        {

            string strselectqry = "nzk0p+jQ+VShKZm/02CtEpbGHgOtdX+lcyt6PIKmYQ/A2i9+N1xdIfDDzMjxf4GPpmbbSOS+WVqPOAPWWXEVeTbXEwtOVzOVtnB+Xpo9z1TvLA3lNRz05Y6tu7szNWWV";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Config.StrDecrypt(strselectqry));
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();

                }

                catch { }

            }

            return "BIOS Maker: Unknown";

        }

        /// <summary>
        /// Retrieving BIOS Serial No.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOSserNo()
        {

            string strselectqry = "mr+SzNcvJHuhH1VSlNCqwTD4Or9Ko9f9nCLAUGWWi9z7m3HNW2sOcoZyhE85S3Q6hNFIDJh1SbFXHUnXW8/hZrkX6SFHR7hX8SC8EGlmX/KLrX34O2396A/OOhtc7XlA";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Config.StrDecrypt(strselectqry));
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("SerialNumber").ToString();

                }

                catch { }

            }

            return "BIOS Serial Number: Unknown";

        }

        /// <summary>
        /// Retrieving BIOS Caption.
        /// </summary>
        /// <returns></returns>
        public static string GetBIOScaption()
        {

            string strselectqry = "0xFkzt0LeHetJk7jwZ7B+Gq88dbkm0PgbtwOjEZmI3V/f9pGi64rydVINSRTfjG2Qve5nar9W48VaZYF48abZkx8g66b8VQHkuTJZpA/obgBFVHpthiUPfK4ayRDOTpz";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Config.StrDecrypt(strselectqry));
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Caption").ToString();

                }
                catch { }
            }
            return "BIOS Caption: Unknown";
        }

        /// <summary>
        /// Retrieving System Account Name.
        /// </summary>
        /// <returns></returns>
        public static string GetAccountName()
        {

            string strselectqry = "jMGldnBCxjfk8OhE/rINwh75bz7nFxj6RUs4HVRjumZZ7TZije6qpXIjv0SdO16L0DJLH1E+d+CO1D0AJmwPG4EEcZcf6W9bdldILn+XwjOwhEadN6rIQ8rZDkCU4MQn";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Config.StrDecrypt(strselectqry));
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {

                    return wmi.GetPropertyValue("Name").ToString();
                }
                catch { }
            }
            return "User Account Name: Unknown";

        }

        public static long GetPhysicalMemory()
        {

            string strselectqry = "JVvf2TYP6QH4O0RSb1uJ/vbx9LCC7VMCbqv/B5EMDOpNEZhM5UytYtyTDv0v12h8NefUE4je2GpWApk9CZlP790+5WxzgGQEBDyjWbWwMplbVIScctZxeIF6Ws8k8B6+FX49cb3Z6vM6EGDUk9R/Pw1cBUesiQ1/4jxgE3mT1aM=";
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery = new ObjectQuery(Config.StrDecrypt(strselectqry));
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oCollection = oSearcher.Get();

            long MemSize = 0;
            long mCap = 0;

            // In case more than one Memory sticks are installed
            foreach (ManagementObject obj in oCollection)
            {
                mCap = Convert.ToInt64(obj["Capacity"]);
                MemSize += mCap;
            }
            MemSize = (MemSize / 1024) / 1024;
            return MemSize;
        }

        public static string GetNoRamSlots()
        {

            int MemSlots = 0;

            string strselectqry = "XBj3CWD5/NieOIftwPWzFOVxh+A+BAHwLiho8kxenbNhjFA1teNKeLcm//jL7uJVMUpXZO2OFHnfyLV2n3iq8IEY1fGT82/FxIlwZLE9KotWZqARwDrpmkflSSDiS8huZcyDIiOGFqRoVGyby0e1oaaeYjyg1Fu/UslGGaEsink=";
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery2 = new ObjectQuery(Config.StrDecrypt(strselectqry));
            ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            ManagementObjectCollection oCollection2 = oSearcher2.Get();
            foreach (ManagementObject obj in oCollection2)
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);

            }
            return MemSlots.ToString();
        }

        //Get CPU Temprature.
        /// <summary>
        /// method for retrieving the CPU Manufacturer
        /// using the WMI class
        /// </summary>
        /// <returns>CPU Manufacturer</returns>
        public static string GetCPUManufacturer()
        {
            string cpuMan = String.Empty;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            ManagementClass mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            ManagementObjectCollection objCol = mgmt.GetInstances();
            //start our loop for all processors found
            foreach (ManagementObject obj in objCol)
            {
                if (cpuMan == String.Empty)
                {
                    // only return manufacturer from first CPU
                    cpuMan = obj.Properties["Manufacturer"].Value.ToString();
                }
            }
            return cpuMan;
        }

        /// <summary>
        /// method to retrieve the CPU's current
        /// clock speed using the WMI class
        /// </summary>
        /// <returns>Clock speed</returns>
        public static int GetCPUCurrentClockSpeed()
        {
            int cpuClockSpeed = 0;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            ManagementClass mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            ManagementObjectCollection objCol = mgmt.GetInstances();
            //start our loop for all processors found
            foreach (ManagementObject obj in objCol)
            {
                if (cpuClockSpeed == 0)
                {
                    // only return cpuStatus from first CPU
                    cpuClockSpeed = Convert.ToInt32(obj.Properties["CurrentClockSpeed"].Value.ToString());
                }
            }
            //return the status
            return cpuClockSpeed;
        }

        /// <summary>
        /// method to retrieve the network adapters
        /// default IP gateway using WMI
        /// </summary>
        /// <returns>adapters default IP gateway</returns>
        public static string GetDefaultIPGateway()
        {
            //create out management class object using the
            //Win32_NetworkAdapterConfiguration class to get the attributes
            //of the network adapter
            ManagementClass mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //create our ManagementObjectCollection to get the attributes with
            ManagementObjectCollection objCol = mgmt.GetInstances();
            string gateway = String.Empty;
            //loop through all the objects we find
            foreach (ManagementObject obj in objCol)
            {
                if (gateway == String.Empty)  // only return MAC Address from first card
                {
                    //grab the value from the first network adapter we find
                    //you can change the string to an array and get all
                    //network adapters found as well
                    //check to see if the adapter's IPEnabled
                    //equals true
                    if ((bool)obj["IPEnabled"] == true)
                    {
                        gateway = obj["DefaultIPGateway"].ToString();
                    }
                }
                //dispose of our object
                obj.Dispose();
            }
            //replace the ":" with an empty space, this could also
            //be removed if you wish
            gateway = gateway.Replace(":", "");
            //return the mac address
            return gateway;
        }

        /// <summary>
        /// Retrieve CPU Speed.
        /// </summary>
        /// <returns></returns>
        public static double? GetCpuSpeedInGHz()
        {
            double? GHz = null;
            using (ManagementClass mc = new ManagementClass("Win32_Processor"))
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    GHz = 0.001 * (UInt32)mo.Properties["CurrentClockSpeed"].Value;
                    break;
                }
            }
            return GHz;
        }
    }
}
