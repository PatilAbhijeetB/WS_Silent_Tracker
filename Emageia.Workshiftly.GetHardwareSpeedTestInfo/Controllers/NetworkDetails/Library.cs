using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Common;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Management;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.HardwareDetails;
using Microsoft.Win32;
using System.ServiceProcess;
using Newtonsoft.Json;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.SystemDetails;
using Emageia.Workshiftly.Entity.ServiceModels;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using System;
using System.ServiceProcess;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.NetworkDetails
{

    public class Library
    {
        private static SpeedTestClient client;
        private static Settings settings;
        public static string udomain = "", uHostName = "", uIpAddress = "", uDlSpeed = "", uUpSpeed = "", uBestServer = "", uPingMs = "", uJitter = "", uWANIp = "", uISP = "";


        /// <summary>
        /// Ping donwload upload jitter wap ip  service provider details
        /// </summary>
        public static BandwidthDetails Bandwidth()
        {
            try
            {
                //     Console.ReadKey();
                string macName = System.Environment.MachineName.ToString();
                // Console.WriteLine("Machine Name  - " + macName.ToString());
                string strHostName = System.Net.Dns.GetHostName();
                string strIp = System.Net.Dns.GetHostAddresses(strHostName).GetValue(0).ToString();

                client = new SpeedTestClient();
                settings = client.GetSettings();

                string LocalIp = string.Empty;
                string Domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                string Host = System.Net.Dns.GetHostName();
                System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (System.Net.IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        LocalIp = ip.ToString();
                        break;
                    }
                }

                udomain = Domain;
                uHostName = Host;
                uIpAddress = LocalIp;
                string IpWidHost = String.Format("\n Domain-{0} \n Host-{1} \n IP-{2}", Domain, Host, LocalIp);
                // Console.WriteLine("\nNetwork Details - " + IpWidHost);
                CommonFunctions.LogWriteLines("\nNetwork Details - " + IpWidHost);
                string Jitter = getjitter();
                CommonFunctions.LogWriteLines("\n\nJitter - " + Jitter);



                //  Console.WriteLine("\n\nISP Name - " + settings.Client.Isp.ToString());
                CommonFunctions.LogWriteLines("\n\nISP Name - " + settings.Client.Isp.ToString());

                uISP = settings.Client.Isp.ToString();
                var wanIP = getWANIP();
                CommonFunctions.LogWriteLines("\n\nWANIP - " + wanIP);
                var Ping = getPing(LocalIp);
                //try
                //{
                var servers = SelectServers();
                var bestServer = SelectBestServer(servers);
                CommonFunctions.LogWriteLines("\n\nPing - " + Ping);
                //   Console.WriteLine("\nTesting speed...");
                CommonFunctions.LogWriteLines("\nTesting speed...");
                var downloadSpeed = client.TestDownloadSpeed(bestServer, settings.Download.ThreadsPerUrl);
                double dnldspeed = Math.Round(downloadSpeed / 1024, 2);
                uDlSpeed = dnldspeed.ToString();
                PrintSpeed("Download", downloadSpeed);
                var uploadSpeed = client.TestUploadSpeed(bestServer, settings.Upload.ThreadsPerUrl);
                double upldspeed = Math.Round(uploadSpeed / 1024, 2);
                uUpSpeed = upldspeed.ToString();
                PrintSpeed("Upload", uploadSpeed);
                //  }
                //catch (Exception ew)
                //{


                //}



                //var bandwidth = Newtonsoft.Json.JsonConvert.SerializeObject(new
                //{
                //    machineName = macName.ToString(),
                //    hostName = System.Net.Dns.GetHostName(),
                //    hostIP = System.Net.Dns.GetHostAddresses(strHostName).GetValue(0).ToString(),
                //    Domain = Domain,
                //    Host = Host,
                //    IP = LocalIp,
                //    Jitter = Jitter,
                //    ISPName = settings.Client.Isp.ToString(),
                //    WANIP = wanIP,
                //    Ping = Ping,
                //    downloadSpeed = PrintSpeed("Download", downloadSpeed),
                //    uploadSpeed = PrintSpeed("Upload", uploadSpeed),
                //    isServer = OSVersion.IsServer


                // });

                BandwidthDetails bandwithDetails = new BandwidthDetails
                {
                    machineName = macName.ToString(),
                    hostName = System.Net.Dns.GetHostName(),
                    hostIP = System.Net.Dns.GetHostAddresses(strHostName).GetValue(0).ToString(),
                    Domain = Domain,
                    Host = Host,
                    IP = LocalIp,
                    Jitter = Jitter,
                    ISPName = settings.Client.Isp.ToString(),
                    WANIP = wanIP,
                    Ping = Ping,
                    downloadSpeed = PrintSpeed("Download", downloadSpeed),
                    uploadSpeed = PrintSpeed("Upload", uploadSpeed),
                    isServer = OSVersion.IsServer
                };




                return bandwithDetails;

            }
            catch (Exception ex)
            {
                BandwidthDetails bandwithDetails = new BandwidthDetails
                {
                    machineName = System.Environment.MachineName.ToString(),
                    hostName = System.Net.Dns.GetHostName(),
                    hostIP = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).GetValue(0).ToString(),
                    Domain = udomain,
                    Host = "",
                    IP = "",
                    Jitter = uJitter,
                    ISPName = "",
                    WANIP = "",
                    Ping = "",
                    downloadSpeed = "",
                    uploadSpeed = "",
                    isServer = OSVersion.IsServer

                };
                //var bandwidth = Newtonsoft.Json.JsonConvert.SerializeObject(new
                //{
                //machineName = System.Environment.MachineName.ToString(),
                //    hostName = System.Net.Dns.GetHostName(),
                //    hostIP = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).GetValue(0).ToString(),
                //    Domain = udomain,
                //    Host = "",
                //    IP = "",
                //    Jitter = uJitter,
                //    ISPName = "",
                //    WANIP = "",
                //    Ping = "",
                //    downloadSpeed = "",
                //    uploadSpeed = "",
                //    isServer = OSVersion.IsServer


                //});
                return bandwithDetails;
                //  InsertLogs(ex.Message.ToString(), "Bandwidth_BandwidthDetails");
            }

        }

        #region Upload | Download Details


        private static Server SelectBestServer(IEnumerable<Server> servers)
        {
            //  Console.WriteLine();
            //   Console.WriteLine("Best server by latency:");
            //  var bestServer = servers.OrderBy(x => x.Latency).First();
            var bestServer = servers.FirstOrDefault();
            PrintServerDetails(bestServer);
            return bestServer;
        }

        private static IEnumerable<Server> SelectServers()
        {

            //Console.WriteLine("Selecting best server by distance...");
            var servers = settings.Servers.Take(10).ToList();
            try
            {
                foreach (var server in servers)
                {
                    server.Latency = client.TestServerLatency(server);
                    // PrintServerDetails(server);
                }
            }
            catch (Exception)
            {

                //  throw;
            }

            return servers;
        }

        /// <summary>
        ///  //  Console.WriteLine("Hosted by {0} ({1}/{2}), distance: {3}km, latency: {4}ms", server.Sponsor, server.Name,
        ///    //  server.Country, (int)server.Distance / 1000, server.Latency);
        /// </summary>
        /// <param name="server"></param>
        private static void PrintServerDetails(Server server)
        {
            try
            {
                uBestServer = server.Sponsor.ToString() + "_" + server.Name.ToString();

            }
            catch (Exception)
            {

                // throw;
            }


        }

        /// <summary>
        /// Console.WriteLine("{0} speed: {1} Mbps", type, Math.Round(speed / 1024, 2))
        ///  Console.WriteLine("{0} speed: {1} Kbps", type, Math.Round(speed, 2));
        /// </summary>
        /// <param name="type"></param>
        /// <param name="speed"></param>
        private static string PrintSpeed(string type, double speed)
        {
            if (speed > 1024)
            {
                //   Console.WriteLine("{0} speed: {1} Mbps", type, Math.Round(speed / 1024, 2));
                var ne = string.Format("{0} speed: {1} Mbps", type, Math.Round(speed / 1024, 2));
                CommonFunctions.LogWriteLines(ne);
                return ne;
            }
            else
            {
                //    Console.WriteLine("{0} speed: {1} Kbps", type, Math.Round(speed, 2));
                var ne2 = string.Format("{0} speed: {1} Kbps", type, Math.Round(speed, 2));
                CommonFunctions.LogWriteLines(ne2);
                return ne2;
            }
        }
        #endregion

        #region Jitter | Ping | WANIP
        private static string getjitter()
        {
            Ping myPing = new Ping();
            List<string> termsList = new List<string>();
            List<double> latlist = new List<double>();

            string wr = "2nqX5zSKGWT+mtJqFARYhEmyIzdAyW827S3fd5QQhfQX+/YhAflRPcHkelHPZEazpeBm08XaO2HcQqUdQsHB5OLbuPIXJIiwxg3sDono+Bx/cAPEFjdxYqAddQQmhZiU";
            string strgoogle = Config.StrDecrypt(wr);

            for (int i = 0; i < 20; i++)
            {
                PingReply reply = myPing.Send(strgoogle, 1000);
                termsList.Add(reply.RoundtripTime.ToString());
            }

            foreach (string lat in termsList)
            {
                lat.Replace("ms", "");

            }
            foreach (string lat in termsList)
            {
                latlist.Add(Math.Abs(Convert.ToDouble(lat)));
            }


            double lataverage = 0;
            List<double> latlist2 = new List<double>();
            for (int i = 1; i < latlist.Count; i++)
            {
                double latval = 0;
                latval = Math.Abs(latlist[i - 1] - latlist[i]);
                latlist2.Add(latval);
            }
            List<double> latlist3 = new List<double>();
            foreach (double lat in latlist2)
            {
                if (Convert.ToDouble(lat) != 0)
                {
                    latlist3.Add(Convert.ToDouble(lat));
                }

            }
            lataverage = latlist3.Average();
            uJitter = lataverage.ToString();
            return uJitter;
            // Console.WriteLine("\nJitter  - " + lataverage.ToString());

        }

        public static string getWANIP()
        {

            String address = "";
            string wr = "nlOIVcttKQK5lZA+jRQKPScAFAX5H2Tgr9E1Qz2U/e8UzNepHC73Kj+pXj0is9FdAIPg/Sjbgk1SB5s/C0vl+Muy+JNXP3HJ3jysbZoESrKyHhT5QgmFMP3cFjfxoLV6";
            WebRequest request = WebRequest.Create(Config.StrDecrypt(wr));
            //WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            // Console.WriteLine("\n\nWAN IP - " + address.ToString());
            uWANIp = address.ToString();
            return uWANIp;

        }

        public static string getPing(string lip)
        {
            Ping pingSender = new Ping();
            IPAddress Paddress = IPAddress.Parse(lip.ToString()); // this should be system ip which is captured above.
            PingReply reply = pingSender.Send(Paddress);

            //Console.WriteLine("\n\nPing Result");

            if (reply.Status == IPStatus.Success)
            {
                //Console.WriteLine(" Address: {0}", reply.Address.ToString());
                //Console.WriteLine(" RoundTrip time: {0}", reply.RoundtripTime);
                //Console.WriteLine(" Time to live: {0}", reply.Options.Ttl);
                //Console.WriteLine(" Don't fragment: {0}", reply.Options.DontFragment);
                //Console.WriteLine(" Buffer size: {0}", reply.Buffer.Length);

                uPingMs = reply.RoundtripTime.ToString();
                return uPingMs;
            }
            else
            {
                return "";
                // Console.WriteLine(reply.Status);
            }


        }



        //public static void LogWriteLines(string fuction)
        //{
        //    try
        //    {
        //        var paths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //        var subFolderPath = System.IO.Path.Combine(paths, "Workshiftly Client");
        //        var subFolder = System.IO.Path.Combine(subFolderPath, "NewFeature.txt");

        //        string Date = "\n \n" + DateTime.Now.ToString();

        //        //string name = string.Format("{0,20}{1,8}{2,18}{3,30}{4,26}",
        //        //        "Date", "Type", "Fuction", "Result", "Error Description");
        //        string name2 = string.Format("{0,20}{1,20}",
        //                Date, fuction);

        //        string[] line = new string[] { name2 };

        //        File.AppendAllLines(subFolder, line);



        //        //Console.WriteLine("{0,26}{1,8}{2,26}",
        //        //        "Argument", "Digits", "Result");


        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        #endregion



        #region HardwareInfo
        [Obsolete]
        public static SystemCaptureDetails GetSystemDetails()
        {
            try
            {

                SysParams sysParams = new SysParams();
                var pageFile = Math.Round(sysParams.getPageFile(), 1);
                var memoryCommite = string.Format("{0} GB", Math.Round(sysParams.getMemCommited() / 1024, 2));
                var MemPoolPaged = string.Format("{0} MB", Math.Round(sysParams.getMemPoolPaged(), 2));
                var MemPoolNonPaged = string.Format("{0} MB", Math.Round(sysParams.getMemPoolNonPaged(), 2));
                var MemCachedBytes = string.Format("{0} MB", Math.Round(sysParams.getMemCachedBytes(), 2));
                var MemAvailable = string.Format("{0} GB", Math.Round(sysParams.getMemAvailable() / 1024, 2));
                // var ne = string.Format("{0} GB", Math.Round(speed / 1024, 2));
                //var pageFiles = Math.Round(pageFile,1);
                //var memoryCommites = string.Format("{0} GB", Math.Round(memoryCommite / 1024, 2));
                //var MemPoolPageds = string.Format("{0} MB", Math.Round(MemPoolPaged, 2));
                //var MemPoolNonPageds = string.Format("{0} MB", Math.Round(MemPoolNonPaged, 2));
                //var MemCachedBytess = string.Format("{0} MB", Math.Round(MemCachedBytes, 2));
                //var MemAvailables = string.Format("{0} GB", Math.Round(MemAvailable / 1024, 2));


                string macName = System.Environment.MachineName.ToString();
                string macFullName = System.Environment.MachineName.ToString() + "." + System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                string sysProcessor = "";
                string sysProcessorID = "";
                float sysRam = 0;
                string sysproductid = "";
                string sysOsVersion = "";
                string sysOsName = "";
                string sysOsArch = "";

                sysProcessorID = HardwareInfo.GetProcessorId();
                sysproductid = HardwareInfo.GetBoardProductId();
                sysProcessor = HardwareInfo.getProcessorName();
                sysRam = (float)HardwareInfo.GetPhysicalMemory();
                sysOsVersion = System.Environment.OSVersion.ToString();


                string strselectqry = "Ui9DPj+7MRR6XZ7ePUkBcRyYGAve3z/9BHpaB4FPghQgPtqc0j1vP7EhBbhPfi3Thsub4UhpImP6YSP3LzTueQn4vRtNZkdMe2T8RdNBXkqLHxA/+SGg+YKVlpT/sBApHLsDgV9iAqcdvK3j1KCNreVGnY+w08XOFzILmb7GmK0=";


                ManagementObjectSearcher mos = new ManagementObjectSearcher(Config.StrDecrypt(strselectqry));
                foreach (ManagementObject managementObject in mos.Get())
                {
                    sysOsName = managementObject["Caption"].ToString();
                    sysOsArch = managementObject["OSArchitecture"].ToString();
                }

                string HardwareDetails = "";
                HardwareDetails = "Machine Name:" + macName + ",";
                HardwareDetails = HardwareDetails + "Machine Full Name:" + macFullName + ",";
                CommonFunctions.LogWriteLines("\n Machine Full Name: " + macFullName);

                HardwareDetails = HardwareDetails + "Processor:" + sysProcessor + ",";
                CommonFunctions.LogWriteLines("\n Processor: " + sysProcessor);

                HardwareDetails = HardwareDetails + "Processor Id:" + sysProcessorID + ",";
                CommonFunctions.LogWriteLines("\n sysProcessorID: " + sysProcessorID);

                HardwareDetails = HardwareDetails + "RAM(MB):" + sysRam + ",";
                CommonFunctions.LogWriteLines("\n RAM(MB): " + sysRam);

                HardwareDetails = HardwareDetails + "Product Id:" + sysproductid + ",";
                CommonFunctions.LogWriteLines("\n sys product id: " + sysproductid);

                HardwareDetails = HardwareDetails + "OS Name:" + sysOsName + ",";
                CommonFunctions.LogWriteLines("\n Processor: " + sysProcessor);

                HardwareDetails = HardwareDetails + "OS Architecture:" + sysOsArch + ",";
                CommonFunctions.LogWriteLines("\n sys Os Name: " + sysOsName);

                HardwareDetails = HardwareDetails + "OS Version:" + sysOsVersion + ",";
                CommonFunctions.LogWriteLines("\n sys Os Version: " + sysOsVersion);

                //Windows Version Details
                HardwareDetails = HardwareDetails + "Windows version:" + $"{OSVersion.GetOSVersion().Version.Major}." + $"{OSVersion.GetOSVersion().Version.Minor}." + $"{OSVersion.GetOSVersion().Version.Build}" + ",";
                CommonFunctions.LogWriteLines("\n Windows version: " + $"{OSVersion.GetOSVersion().Version.Major}." + $"{OSVersion.GetOSVersion().Version.Minor}." + $"{OSVersion.GetOSVersion().Version.Build}" + ",");

                HardwareDetails = HardwareDetails + "OS type:" + OSVersion.GetOperatingSystem() + ",";
                CommonFunctions.LogWriteLines("\n OS type: " + OSVersion.GetOperatingSystem() + ",");

                HardwareDetails = HardwareDetails + "Is workstation:" + OSVersion.IsWorkstation + ",";
                CommonFunctions.LogWriteLines("\n Is workstation: " + OSVersion.IsWorkstation + ",");

                HardwareDetails = HardwareDetails + "Is server:" + OSVersion.IsServer + ",";
                CommonFunctions.LogWriteLines("\n Is server: " + OSVersion.IsServer + ",");
                // HardwareDetails = HardwareDetails + "64-Bit OS" + OSVersion.Is64BitOperatingSystem + ",";

                var WindowsReleaseID = string.Empty;
                var WindowsDisplayVersion = string.Empty;
                var WindowsUpdateBuildRevision = string.Empty;

                if (OSVersion.GetOSVersion().Version.Major >= 10)
                {
                    HardwareDetails = HardwareDetails + $"Windows Release ID: {OSVersion.MajorVersion10Properties().ReleaseId ?? "(Unable to detect)"}" + ",";
                    CommonFunctions.LogWriteLines($"\n Windows Release ID: {OSVersion.MajorVersion10Properties().ReleaseId ?? "(Unable to detect)"}" + ",");


                    HardwareDetails = HardwareDetails + $"Windows Display Version: {OSVersion.MajorVersion10Properties().DisplayVersion ?? "(Unable to detect)"}" + ",";
                    CommonFunctions.LogWriteLines($"\nWindows Display Version: {OSVersion.MajorVersion10Properties().DisplayVersion ?? "(Unable to detect)"} " + ",");

                    HardwareDetails = HardwareDetails + $"Windows Update Build Revision: {OSVersion.MajorVersion10Properties().UBR ?? "(Unable to detect)"}";
                    CommonFunctions.LogWriteLines($"\nWindows Update Build Revision: {OSVersion.MajorVersion10Properties().UBR ?? "(Unable to detect)"}");

                    WindowsReleaseID = OSVersion.MajorVersion10Properties().ReleaseId ?? "(Unable to detect)";

                    WindowsDisplayVersion = OSVersion.MajorVersion10Properties().DisplayVersion ?? "(Unable to detect)";
                    WindowsUpdateBuildRevision = OSVersion.MajorVersion10Properties().UBR ?? "(Unable to detect)";

                }


                // Savetodb(HardwareDetails, strMonDetails);
                //    var systemDetails = Newtonsoft.Json.JsonConvert.SerializeObject(new
                //    {
                //        machineName = macName,
                //        machineFullName = macFullName,
                //        processor = sysProcessor,
                //        processorId = sysProcessorID,
                //        ram = sysRam,
                //        productId = sysproductid,
                //        osName = sysOsName,
                //        osArchitecture = sysOsArch,
                //        osVersion = sysOsVersion,
                //        windowsversion = $"{OSVersion.GetOSVersion().Version.Major}." + $"{OSVersion.GetOSVersion().Version.Minor}." + $"{OSVersion.GetOSVersion().Version.Build}",
                //        oStype = OSVersion.GetOperatingSystem(),
                //        isWorkstation = OSVersion.IsWorkstation,
                //        isServer = OSVersion.IsServer,
                //        windowsReleaseID = WindowsReleaseID,
                //        windowsDisplayVersion = WindowsDisplayVersion,
                //        windowsUpdateBuildRevision = WindowsUpdateBuildRevision,
                //        pageFile= pageFile,
                //        memoryCommite = memoryCommite,
                //        memoryPoolPaged = MemPoolPaged,
                //        memoryPoolNonPaged = MemPoolNonPaged,
                //        memoryCachedBytes = MemCachedBytes,
                //        memoryAvailable = MemAvailable,

                //});
                var systemDetails = new SystemCaptureDetails
                {
                    machineName = macName,
                    machineFullName = macFullName,
                    processor = sysProcessor,
                    processorId = sysProcessorID,
                    ram = string.Format("{0} GB", Math.Round(sysRam / 1024, 2)),
                    productId = sysproductid,
                    osName = sysOsName,
                    osArchitecture = sysOsArch,
                    osVersion = sysOsVersion,
                    windowsVersion = $"{OSVersion.GetOSVersion().Version.Major}." + $"{OSVersion.GetOSVersion().Version.Minor}." + $"{OSVersion.GetOSVersion().Version.Build}",
                    oStype = OSVersion.GetOperatingSystem().ToString(),
                    isWorkstation = OSVersion.IsWorkstation,
                    isServer = OSVersion.IsServer,
                    windowsReleaseId = WindowsReleaseID,
                    windowsDisplayVersion = WindowsDisplayVersion,
                    windowsUpdateBuildRevision = WindowsUpdateBuildRevision,
                    pageFile = (float)pageFile,
                    memoryCommite = memoryCommite,
                    memoryPoolPaged = MemPoolPaged,
                    memoryPoolNonPaged = MemPoolNonPaged,
                    memoryCachedBytes = MemCachedBytes,
                    memoryAvailable = MemAvailable,
                };


                // var strMonDetails = getMonitorDetails();
                return systemDetails;
            }
            catch (Exception ex)
            {
                return new SystemCaptureDetails();
                //  InsertLogs(ex.Message.ToString(), "HardwareInfo_GetSystemDetails");
            }
        }



        public static List<monitorInfo> getMonitorDetails()
        {
            try
            {
                SelectQuery Sq = new SelectQuery("Win32_DesktopMonitor");
                ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
                ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
                StringBuilder sb = new StringBuilder();
                string mondetails = "";
                List<monitorInfo> moniters = new List<monitorInfo>(osDetailsCollection.Count);
                // string[] moniters = new string[osDetailsCollection.Count];

                CommonFunctions.LogWriteLines("\n get Monitor Details  ****************************************");
                foreach (ManagementObject mo in osDetailsCollection)
                {

                    mondetails = "Name :" + Convert.ToString(mo["Name"]) + ",";
                    CommonFunctions.LogWriteLines("\nName :" + Convert.ToString(mo["Name"]) + ",");


                    mondetails = mondetails + "Availability :" + Convert.ToString(mo["Availability"]) + ",";
                    CommonFunctions.LogWriteLines("\nAvailability :" + Convert.ToString(mo["Availability"]) + ",");


                    mondetails = mondetails + "Caption :" + Convert.ToString(mo["Caption"]) + ",";
                    CommonFunctions.LogWriteLines("\nCaption :" + Convert.ToString(mo["Caption"]));


                    mondetails = mondetails + "InstallDate :" + Convert.ToDateTime(mo["InstallDate"]) + ",";
                    CommonFunctions.LogWriteLines("\nInstallDate :" + Convert.ToDateTime(mo["InstallDate"]) + ",");


                    mondetails = mondetails + "ConfigManagerUserConfig :" + Convert.ToString(mo["ConfigManagerUserConfig"]) + ",";
                    CommonFunctions.LogWriteLines("\nConfigManagerUserConfig :" + Convert.ToString(mo["ConfigManagerUserConfig"]) + ",");


                    mondetails = mondetails + "CreationClassName :" + Convert.ToString(mo["CreationClassName"]) + ",";
                    CommonFunctions.LogWriteLines("\nCreationClassName :" + Convert.ToString(mo["CreationClassName"]) + ",");


                    mondetails = mondetails + "Description :" + Convert.ToString(mo["Description"]) + ",";
                    CommonFunctions.LogWriteLines("\nDescription :" + Convert.ToString(mo["Description"]) + ",");


                    mondetails = mondetails + "DeviceID :" + Convert.ToString(mo["DeviceID"]) + ",";
                    CommonFunctions.LogWriteLines("\nDeviceID :" + Convert.ToString(mo["DeviceID"]) + ",");


                    mondetails = mondetails + "ErrorCleared :" + Convert.ToString(mo["ErrorCleared"]) + ",";
                    CommonFunctions.LogWriteLines("\nErrorCleared :" + Convert.ToString(mo["ErrorCleared"]) + ",");


                    mondetails = mondetails + "ErrorDescription :" + Convert.ToString(mo["ErrorDescription"]) + ",";
                    CommonFunctions.LogWriteLines("\nErrorDescription :" + Convert.ToString(mo["ErrorDescription"]) + ",");


                    mondetails = mondetails + "ConfigManagerUserConfig :" + Convert.ToString(mo["ConfigManagerUserConfig"]) + ",";
                    CommonFunctions.LogWriteLines("\nConfigManagerUserConfig :" + Convert.ToString(mo["ConfigManagerUserConfig"]) + ",");



                    mondetails = mondetails + "LastErrorCode :" + Convert.ToString(mo["LastErrorCode"]) + ",";
                    CommonFunctions.LogWriteLines("\nLastErrorCode :" + Convert.ToString(mo["LastErrorCode"]) + ",");


                    mondetails = mondetails + "MonitorManufacturer :" + Convert.ToString(mo["MonitorManufacturer"]) + ",";
                    CommonFunctions.LogWriteLines("\nMonitorManufacturer :" + Convert.ToString(mo["MonitorManufacturer"]) + ",");


                    mondetails = mondetails + "PNPDeviceID :" + Convert.ToString(mo["PNPDeviceID"]) + ",";
                    CommonFunctions.LogWriteLines("\nPNPDeviceID :" + Convert.ToString(mo["PNPDeviceID"]) + ",");


                    mondetails = mondetails + "MonitorType :" + Convert.ToString(mo["MonitorType"]) + ",";
                    CommonFunctions.LogWriteLines("\nMonitorType :" + Convert.ToString(mo["MonitorType"]) + ",");


                    mondetails = mondetails + "PixelsPerXLogicalInch :" + Convert.ToString(mo["PixelsPerXLogicalInch"]) + ",";
                    CommonFunctions.LogWriteLines("\nPixelsPerXLogicalInch :" + Convert.ToString(mo["PixelsPerXLogicalInch"]) + ",");


                    mondetails = mondetails + "PixelsPerYLogicalInch :" + Convert.ToString(mo["PixelsPerYLogicalInch"]) + ",";
                    CommonFunctions.LogWriteLines("\nPixelsPerYLogicalInch :" + Convert.ToString(mo["PixelsPerYLogicalInch"]) + ",");


                    mondetails = mondetails + "Status :" + Convert.ToString(mo["Status"]) + ",";
                    CommonFunctions.LogWriteLines("\nStatus :" + Convert.ToString(mo["Status"]) + ",");


                    mondetails = mondetails + "SystemCreationClassName :" + Convert.ToString(mo["SystemCreationClassName"]) + ",";
                    CommonFunctions.LogWriteLines("\nSystemCreationClassName :" + Convert.ToString(mo["SystemCreationClassName"]) + ",");


                    mondetails = mondetails + "SystemName :" + Convert.ToString(mo["SystemName"]) + ",";
                    CommonFunctions.LogWriteLines("\nSystemName :" + Convert.ToString(mo["SystemName"]) + ",");


                    var moniter = new monitorInfo();
                    moniter.dataCreatedTime = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                    moniter.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;

                    moniter.monitorName = Convert.ToString(mo["Name"]);
                    // moniter.Availability = Convert.ToString(mo["Availability"]),
                    moniter.caption = Convert.ToString(mo["Caption"]);
                    moniter.installDate = Convert.ToDateTime(mo["InstallDate"]).ToString();
                    //    ConfigManagerUserConfig = Convert.ToString(mo["ConfigManagerUserConfig"]),
                    moniter.creationClassName = Convert.ToString(mo["CreationClassName"]);
                    //   Description = Convert.ToString(mo["Description"]),
                    moniter.hardwareDeviceId = Convert.ToString(mo["DeviceID"]);
                    // ErrorCleared = Convert.ToString(mo["ErrorCleared"]),
                    // ErrorDescription = Convert.ToString(mo["ErrorDescription"]),
                    //   LastErrorCode = Convert.ToString(mo["LastErrorCode"]),
                    moniter.monitorManufacture = Convert.ToString(mo["MonitorManufacturer"]);
                    moniter.pnpDeviceId = Convert.ToString(mo["PNPDeviceID"]);
                    moniter.monitorType = Convert.ToString(mo["MonitorType"]);
                    moniter.pixelsPerXLogicalInch = Convert.ToString(mo["PixelsPerXLogicalInch"]);
                    moniter.pixelsPerYLogicalInch = Convert.ToString(mo["PixelsPerYLogicalInch"]);
                    moniter.status = Convert.ToString(mo["Status"]);
                    moniter.systemCreationClassName = Convert.ToString(mo["SystemCreationClassName"]);
                    moniter.systemName = Convert.ToString(mo["SystemName"]);



                    moniters.Add(moniter);
                    // moniters.Append(moniter);
                }

                // return mondetails.ToString();
                return moniters;
            }
            catch (Exception ex)
            {
                // InsertLogs(ex.Message.ToString(), "HardwareInfo_getMonitorDetails");
                return new List<monitorInfo>();
            }

        }
        #endregion

        #region SoftwareDetailse
        public static List<SoftwareDetails> GetSoftwareDetails()
        {
            try
            {

                string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                string SoftwareDetails = "";
                List<SoftwareDetails> SoftwareDetailsList = new List<SoftwareDetails>();
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
                {
                    CommonFunctions.LogWriteLines("\n get Software Details  ****************************************");
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(skName))
                        {
                            try
                            {

                                var displayName = sk.GetValue("DisplayName");
                                var size = sk.GetValue("EstimatedSize");

                                // ListViewItem item;
                                if (displayName != null)
                                {
                                    // Console.WriteLine("\n" + displayName.ToString());
                                    SoftwareDetails = SoftwareDetails + displayName.ToString() + ",";
                                    CommonFunctions.LogWriteLines("\nDisplayName :" + displayName.ToString() + "\tEstimatedSize :" + size + ",");
                                    var software = JsonConvert.SerializeObject(new
                                    {
                                        displayName = displayName.ToString(),
                                        size = size
                                    });

                                    var softwareDetails = new SoftwareDetails
                                    {
                                        displayName = displayName.ToString(),
                                        size = size == null ? "" : size.ToString(),
                                    };
                                    SoftwareDetailsList.Add(softwareDetails);

                                }

                            }
                            catch (Exception ex)
                            {
                                // SoftwareDetailsList.Add("");
                                // InsertLogs(ex.Message.ToString(), "SoftwareInfo_GetSoftwareDetails");
                            }
                        }



                    }
                    return SoftwareDetailsList;
                    //  Savetodb(SoftwareDetails);

                }


            }
            catch (Exception ex)
            {
                return new List<SoftwareDetails>();
                // InsertLogs(ex.Message.ToString(), "SoftwareInfo_GetSoftwareDetails");
            }
        }



        public static List<ServiceDeatails> getServiceDetails()
        {
            try
            {
                ServiceController[] scServices;
                scServices = ServiceController.GetServices();

                CommonFunctions.LogWriteLines("\n*************** get Service Details  ****************************************");
                List<ServiceDeatails> ServiceDetailsList = new List<ServiceDeatails>();
                string strservice = "";
                string strserviceStatus = "";
                foreach (ServiceController scTemp in scServices)
                {

                    strservice = strservice + scTemp.ServiceName + ",";
                    strserviceStatus = strserviceStatus + scTemp.ServiceName + ":" + scTemp.Status + ",";

                    CommonFunctions.LogWriteLines("\nServiceName :" + scTemp.ServiceName.ToString() + "\tService Status :" + scTemp.Status + ",");

                    var services = new ServiceDeatails();
                    services.serviceName = scTemp.ServiceName.ToString();
                    services.serviceStatus = scTemp.Status.ToString();

                    string startName = "";
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(
               $"SELECT StartName FROM Win32_Service WHERE Name = '{services.serviceName}'");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        startName = queryObj["StartName"] as string;
                        services.logOnAs = startName;
                    }

                    var service = JsonConvert.SerializeObject(new
                    {
                        serviceName = scTemp.ServiceName.ToString(),
                        serviceStatus = scTemp.Status.ToString(),
                        logOnAs = startName


                    }); ;

                    ServiceDetailsList.Add(services);
                }

                return ServiceDetailsList;
                // Savetodb(strservice, strserviceStatus);
            }
            catch (Exception ex)
            {
                return new List<ServiceDeatails>();
                // InsertLogs(ex.Message.ToString(), "ServiceInfo_GetServiceDetails");
            }
        }
        #endregion



    }
}
