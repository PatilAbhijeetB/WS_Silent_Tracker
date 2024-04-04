using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Emageia.Workshiftly.CoreFunction.IoC.Service.Http;
using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow;
using System.Xml;
using Emageia.Workshiftly.Entity;
using Newtonsoft.Json;
using Emageia.Workshiftly.Domain.Concrete;
using System.Linq;
using Emageia.Workshiftly.CoreFunction.IoC.HalAccess;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using System.Threading;
using Emageia.Workshiftly.CoreFunction.IoC.Controllers.IdleTime;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Emageia.Workshiftly.Entity.ServiceModels;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.NetworkDetails;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.SystemDetails;
using System.Device.Location;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Windows.Controls;
using System.Timers;
using Emageia.Workshiftly.ProcessExtensionsService;
using System.Threading.Tasks;
using System.Globalization;
using Emageia.Workshiftly.CoreFunction.IoC.SocketIoClient;
using Emageia.Workshiftly.CoreFunction.IoC.Service.SocketServer;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Emageia.Workshiftly.MainApplication
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> ,IClientDataStore
    public partial class MainWindow : Window
    {
        #region  Prevent windos shutdown

        public const int WM_QUERYENDSESSION = 0x0011;
        public const int WM_ENDSESSION = 0x0016;
        public const uint SHUTDOWN_NORETRY = 0x00000001;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);
        [DllImport("kernel32.dll")]
        static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);

        #endregion
        int h, m, s;
        private ScreenCaptureActiveWindow sc;
        private String CurrentAppVersion = "2.6.0";
        string XMLPath = "";
        //Production Api
        //public string apiUse = "https://portal.workshiftly.com/api";
        //Staging Api
        //public string apiUse = "https://qa.workshiftly.com/api";
       // public string apiUse = "https://staging-api.workshiftly.com/api";
         public string apiUse = "https://portal.workshiftly.com/api";
      
        private static string latitude;
        private static string longitude;
        private static GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

        private Thread Worker = null;
        int isvalid = 0;
        bool first = false;
        public MainWindow()
        {
            InitializeComponent();
            ShowNotification("Service Status", "Services Started");
            Microsoft.Win32.SystemEvents.SessionEnding += new Microsoft.Win32.SessionEndingEventHandler(SystemEvents_SessionEnding);
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(OnPowerModeChanged);

            this.Visibility = Visibility.Hidden;

          
          


            #region path variable
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var paths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            XMLPath = System.IO.Path.Combine(paths, "source.xml");
         

            CommonUtility.SystemBinUrl = AppDomain.CurrentDomain.BaseDirectory;

            #endregion

            #region loging

            try
            {

                if (!IsLogeActiveUser())
                {
                    LogingXmlFileInitiate();
                }
                else
                {
                    Int32 actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                    Int32 companyStartShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift);
                    Int32 companyEndShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.endShift);
                    if (CommonUtility.CompanyConfigurations.isTrackWithinShift && (companyStartShift <= actionTimestamp) && (companyEndShift >= actionTimestamp))
                    {
                        LogingXmlFileInitiate();
                    }
                    else if (!CommonUtility.CompanyConfigurations.isTrackWithinShift)
                    {
                        LogingXmlFileInitiate();
                    }
                }

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Catch", "MainWindow File EXists", "Xml Fill  Error  ", ex.Message.ToString());

            }





            #endregion

            #region checking login

            try
            {
                var _activeTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(10)
                };

                _activeTimer.Tick += delegate (object sender, EventArgs e)
                {
                    if (!IsLogeActiveUser())
                    {
                        LogingXmlFileInitiate();
                    }
                    else
                    {
                        Int32 actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        Int32 companyStartShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift);
                        Int32 companyEndShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.endShift);
                        if (CommonUtility.CompanyConfigurations.isTrackWithinShift && (companyStartShift <= actionTimestamp) && (companyEndShift >= actionTimestamp))
                        {
                            LogingXmlFileInitiate();
                        }
                        else if (!CommonUtility.CompanyConfigurations.isTrackWithinShift)
                        {
                            LogingXmlFileInitiate();
                        }
                    }



                };
                _activeTimer.Start();

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Catch", "MainWindow checking login", "Auto Run Windows Loggin Error  ", ex.Message.ToString());

            }

            #endregion

            #region Initial Loading

            try
            {
                // SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerModeChanged);
                Window_Loaded();

                ThreadStart start = new ThreadStart(Working);
                Worker = new Thread(start);
                Worker.Start();

                //GetSystemInfo();
                //Thread InstallClient = new Thread(async () => await GetSystemInfo());
                //InstallClient.Start();

                ActiveWindowUtilitys();

                SyncAtivityWindows();


                ScreenCaptureUtility();
                // StatusUpdate();

                //Thread capture = new Thread(() => HttpRequestCaller.syncScreenshots());
                //capture.Start();

                //Thread callPostSyncWorkStatusLogs = new Thread(() => HttpRequestCaller.CallPostSyncWorkStatusLogs());
                //callPostSyncWorkStatusLogs.Start();

                // SocketIOAdapter var = new SocketIOAdapter();
                //  CommonUtility.socketIOAdapter = new SocketIOAdapter();
            }
            catch (Exception ex)
            {
            }

            #endregion


        }



        public void ShowNotification(string title, string message)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;
            notifyIcon.Icon = SystemIcons.Information;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            //  notifyIcon.BalloonTipIcon = icon;
            notifyIcon.ShowBalloonTip(5000); // Display for 5 seconds
        }
        #region

        private static readonly HttpClient client = new HttpClient();
        public class ApiResponse
        {
            public string LatestVersion { get; set; }
            public string DownloadLink { get; set; }
        }

        public ApiResponse SendPostRequest(string url, string content)
        {
            try
            {
                url = url + "?current-version=" + content;
                // Send the GET request synchronously
                HttpResponseMessage response = client.GetAsync(url).Result;

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content synchronously
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    JObject responseObject = JObject.Parse(responseContent);

                    // Access the 'data' object and then 'latestVersion' property
                    string latestVersion = responseObject["data"]["latestVersion"].ToString();
                    string additionalString = responseObject["data"]["downloadURLs"]["windows"].ToString();

                    return new ApiResponse { LatestVersion = latestVersion, DownloadLink = additionalString };
                }
                else
                {
                    string errorStatusCode = response.StatusCode.ToString();
                    Console.WriteLine("Error: " + response.StatusCode);
                    // Handle the error or throw an exception
                    return new ApiResponse { LatestVersion = errorStatusCode, DownloadLink = "" };
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                Console.WriteLine("Exception: " + ex.Message);
                // Handle the exception appropriately
                return new ApiResponse { LatestVersion = errorMessage, DownloadLink = "" };
            }
        }
      
        private async void Working()
        {
            try
            {
                int hr = 0;
                string ampm = "";

                //  GetSystemInfo();

                while (true)
                {

                    try
                    {
                        //Get Latest App version on server
                       
                       // ApiResponse response = SendPostRequest("https://qa.workshiftly.com/api/silent-track-latest-version", CurrentAppVersion);
                       // ApiResponse response = SendPostRequest("https://staging-api.workshiftly.com/api/silent-track-latest-version", CurrentAppVersion);
                         ApiResponse response = SendPostRequest("https://portal.workshiftly.com/api/silent-track-latest-version", CurrentAppVersion);
                       

                        String strAppVersionOnServer =response.LatestVersion.ToString();
                        String strDownloadlink=response.DownloadLink.ToString();
                        if (CurrentAppVersion != strAppVersionOnServer)
                        {
                            MessageBoxResult result = System.Windows.MessageBox.Show("New Update is available please confirm to download and update?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            // Check the user's response
                            if (result == MessageBoxResult.Yes)
                            {
                                downloadandinstallUpdate(strDownloadlink);
                            }
                        }
                        else
                        {
                            await GetSystemInfo();
                        }



                        await Task.Delay(36000);


                        //  GetSystemInfo();
                        //hr = Convert.ToInt32(DateTime.Now.Hour);
                        //ampm = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);
                        //// if (hr == 1 && ampm == "AM" && isvalid == 0)
                        //if (hr == 10 && ampm == "AM" && isvalid == 0)
                        //{

                        //    isvalid = 5;
                        //}
                        //else if (hr == 9 && ampm == "AM" && isvalid == 1)
                        //{
                        //    GetSystemInfo();
                        //    isvalid = 2;
                        //}
                        //else if (hr == 14 || hr == 2 && ampm == "PM" && isvalid == 2)
                        //{
                        //    GetSystemInfo();
                        //    isvalid = 3;
                        //}

                        //else if (hr == 20 || hr == 8 && ampm == "PM" && isvalid == 3)
                        //{
                        //    GetSystemInfo();
                        //    isvalid = 0;
                        //}

                        //   GetSystemInfo();

                    }
                    catch (Exception es)
                    {

                        //  throw;
                    }




                    //  Thread.Sleep(100000);
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void downloadandinstallUpdate(String Url)
        {
            //try
            //{
            //    if ((Worker != null) & Worker.IsAlive)
            //    {
            //        Worker.Abort();
            //    }
            //}
            //catch
            //{
            //    throw;
            //}

           // string url = ""; // Assuming txtUrl is a TextBox where the user enters the URL
            string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Specify the file name and extension
            string fileName = "WS_Service.msi"; // Change this to the desired file name and extension

            // Combine the downloads folder path with the file name
            string savePath = Path.Combine(downloadsFolder, fileName);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(Url), savePath);

                // File download complete, execute the downloaded file
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = savePath;
                Process.Start(startInfo);

              //  System.Windows.MessageBox.Show("File downloaded successfully!");
            }
        }
        #endregion
        private async void OnStop()
        {
            try
            {

                if ((Worker != null) & Worker.IsAlive)
                {
                    Worker.Abort();
                }



            }
            catch
            {
                throw;
            }

        }
        private async Task GetSystemInfo()
        {
            try
            {
                LogingXmlFileInitiate();
                ShowStatusUpdates();
                var bandWidth = Library.Bandwidth();


                Utilization utilization = new Utilization();
                UtilizationDetails utilizationsDetails = utilization.getInitCouters();

                // CommonFunctions.LogWriteLines("\n utilization ****************************************************** ");

                // //SysParams sysParams = new SysParams();
                // //sysParams.detailsMemory();
                //// sysParams.initNetCounters();

                var systemDetails = Library.GetSystemDetails();
                var softwareDetails = Library.GetSoftwareDetails();
                var getServiceDetails = Library.getServiceDetails();

                var location = new LocationCoordinates
                {
                    latitude = latitude,
                    longitude = longitude,
                };

                syncDate2(bandWidth, utilizationsDetails, systemDetails, softwareDetails, getServiceDetails, location);

            }
            catch (Exception eq)
            {

                throw;
            }

        }


        private void syncDate2(BandwidthDetails bandWidth, UtilizationDetails utilizationsDetails, SystemCaptureDetails systemDetails, List<SoftwareDetails> softwareDetails, List<ServiceDeatails> getServiceDetails, LocationCoordinates location)
        {
            //var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYzMTAyYjI3LTVhYzctNGY5My04NzA2LTA0NGVmMDBhZWUxNyIsImVtYWlsIjoic2lyaXNlbmFAbWFpbGluYXRvci5jb20iLCJmaXJzdE5hbWUiOiJLdW1hcmEiLCJsYXN0TmFtZSI6IlNpcmlzZW5hIiwicm9sZSI6eyJpZCI6IjIxYmRmYTM1LTIyY2UtNDQyNi04NzMwLWZjNzRlZjhjZDY0OCIsIm5hbWUiOiJVU0VSIiwiZGVzY3JpcHRpb24iOiJ1c2VyIiwicGVybWlzc2lvbnMiOiJ7XCJrZXlcIjpcInVzZXJcIiwgXCJncmFudFR5cGVcIjogXCJhbGxcIiwgXCJkZXNjcmlwdGlvblwiOiBcIlwifSIsImlzQWN0aXZlIjoxLCJjcmVhdGVkQXQiOjE2MjA2NDMxNDQsInVwZGF0ZWRBdCI6MTYyMDY0MzE0NH0sImlzQWN0aXZlIjp0cnVlLCJpc0NsaWVudEFjdGl2ZSI6dHJ1ZSwiaXNBbGxvd09mZmxpbmVUYXNrIjp0cnVlLCJpYXQiOjE2NzA5OTAzOTYsImV4cCI6MTY3NDU5MDM5Nn0._Dfx5SFMrtSIxx0WNt_XVbVkVpWVkhIEoaLBSY2ie0I";
            var token = CommonUtility.UserSessions.authToken;

            using (HttpClient client = new HttpClient())
            {
              //  client.BaseAddress = new Uri("https://qa.workshiftly.com/api/");
             //   client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api/");
               client.BaseAddress = new Uri("https://portal.workshiftly.com/api/");
                // client.BaseAddress = new Uri(apiUse);

                client.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



                /////////////////////////Bandwidth/////////////////////////////////////////
                string dnldspeed = bandWidth.downloadSpeed.ToString();
                string upldspeed = bandWidth.uploadSpeed.ToString();

                dnldspeed = dnldspeed.Replace("Download speed:", "");
                upldspeed = upldspeed.Replace("Upload speed:", "");

                var bwdata = new bandwidthData
                {
                    dataCreatedTime = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
                    deviceId = CommonUtility.LoggedDeviceSettings.deviceID,
                    ispName = bandWidth.ISPName.ToString(),

                    downloadSpeed = dnldspeed,
                    uploadSpeed = upldspeed,
                    jitter = bandWidth.Jitter.ToString(),
                    ping = bandWidth.Ping.ToString(),
                };
                var jbandwidth = JsonConvert.SerializeObject(bwdata);
                StringContent bandwidthJson = new StringContent(jbandwidth, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage bandwidth_response = client.PostAsync("users/" + CommonUtility.UserSessions.id + "/specification-monitor/network/", bandwidthJson).Result;

                }
                catch (Exception ex)
                {

                }
                ///////////////////////////// End BandWidth///////////////////////////////////
                /// /////////////////////////Services/////////////////////////////////////////
                var sedata = new serviceData
                {
                    dataCreatedTime = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
                    deviceId = CommonUtility.LoggedDeviceSettings.deviceID,
                    services = getServiceDetails,
                };
                var jservice = JsonConvert.SerializeObject(sedata);
                StringContent serviceJson = new StringContent(jservice, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage Service_response = client.PostAsync("users/" + CommonUtility.UserSessions.id + "/specification-monitor/system-services/", serviceJson).Result;

                }
                catch (Exception ex)
                {

                }
                ///////////////////////////// End Services ///////////////////////////////////
                ///////////////////////////// System Info /////////////////////////////////////
                var sysdata = new systemInfo
                {
                    dataCreatedTime = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
                    deviceId = CommonUtility.LoggedDeviceSettings.deviceID,
                    machineName = systemDetails.machineName,
                    machineFullName = systemDetails.machineFullName,
                    processor = systemDetails.processor,
                    processorId = systemDetails.processorId,
                    ram = systemDetails.ram,
                    osName = systemDetails.osName,
                    osArchitecture = systemDetails.osArchitecture,
                    osVersion = systemDetails.osVersion,
                    windowsVersion = systemDetails.windowsVersion,
                    osType = systemDetails.oStype,
                    windowsReleaseId = systemDetails.windowsReleaseId,
                    windowsDisplayVersion = systemDetails.windowsDisplayVersion,
                    windowsUpdateBuildRevision = systemDetails.windowsUpdateBuildRevision,


                };
                var jsystem = JsonConvert.SerializeObject(sysdata);
                StringContent sysJson = new StringContent(jsystem, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage Sys_response = client.PostAsync("users/" + CommonUtility.UserSessions.id + "/specification-monitor/system-details/", sysJson).Result;

                }
                catch (Exception ex)
                {

                }
                /////////////////////////////End System Info /////////////////////////////////////
                //////////////////////////////// Monitor Info /////////////////////////////////////

                List<monitorInfo> monitorDetails = Library.getMonitorDetails();

                foreach (monitorInfo monInfo in monitorDetails)
                {
                    var jmoni = JsonConvert.SerializeObject(monInfo);
                    StringContent moniJson = new StringContent(jmoni, Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage Sys_response = client.PostAsync("users/" + CommonUtility.UserSessions.id + "/specification-monitor/monitor-details/", moniJson).Result;

                    }
                    catch (Exception ex)
                    {

                    }

                }
                /////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////

                double dlatitude = 40.7128; // Example latitude
                double dlongitude = -74.0060; // Example longitude

                dlatitude = Convert.ToDouble(location.latitude);
                dlongitude = Convert.ToDouble(location.longitude);

                LocationInfo locationInfo = GetLocationInfo(dlatitude, dlongitude);

                if (locationInfo != null)
                {
                    var locationdata = new geoLocation
                    {
                        dataCreatedTime = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
                        deviceId = CommonUtility.LoggedDeviceSettings.deviceID,
                        latitude = Convert.ToString(dlatitude),
                        longitude = Convert.ToString(dlongitude),
                        city = locationInfo.CityName.ToString(),
                        country = locationInfo.CountryName.ToString(),
                        state = locationInfo.StateName.ToString(),
                        timezone = locationInfo.TimeZone.ToString(),



                    };
                    var locData = JsonConvert.SerializeObject(locationdata);
                    StringContent locJson = new StringContent(locData, Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage Sys_response = client.PostAsync("users/" + CommonUtility.UserSessions.id + "/specification-monitor/location/", locJson).Result;

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    Console.WriteLine("Error retrieving location information.");
                }


            }
        }

        private static void ShowStatusUpdates()
        {
            watcher = new GeoCoordinateWatcher();


            // watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.StatusChanged += watcher_StatusChanged;
            watcher.Start();
            //Console.WriteLine("Enter any key to quit.");
            //Console.ReadLine();
        }

        private static void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {

            try
            {
                if (e.Status == GeoPositionStatus.Ready)
                {
                    if (watcher.Position.Location.IsUnknown)
                    {
                        latitude = "0";
                        longitude = "0";

                    }
                    else
                    {
                        latitude = watcher.Position.Location.Latitude.ToString();
                        longitude = watcher.Position.Location.Longitude.ToString();
                    }
                }
                else
                {
                    latitude = "0";
                    longitude = "0";
                }
            }
            catch (Exception ex)
            {
                latitude = "0";
                longitude = "0";

            }

        }

        private void LogingXmlFileInitiate()
        {
            if (File.Exists(XMLPath))
            {
                logout(); // this logout comment duo to check with recovery
                AutoRunWindowsLoggin();
            }
            if (!IsLogeActiveUser())
            {
                LogingCredentialsUpdate();
            }
        }

        #region Session Ending and power mode and session Lock
        public static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    los("PowerMode: OS is resuming from suspended state");

                    //  MessageBox.Show("PowerMode: OS is resuming from suspended state");
                    break;

                case PowerModes.Suspend:
                    los("PowerMode: OS is about to be suspended");

                    // MessageBox.Show("PowerMode: OS is about to be suspended");
                    break;
            }
        }
        static void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            try
            {

                switch (e.Reason)
                {
                    case Microsoft.Win32.SessionEndReasons.Logoff:
                        los("LogoutUser SystemEvents_SessionEnding called on Logoff");

                        break;

                    case Microsoft.Win32.SessionEndReasons.SystemShutdown:
                        los("LogoutUser SystemEvents_SessionEnding called on Logoff");

                        break;
                }
            }
            catch (Exception ex)
            {
                los("LogoutUser SystemEvents_SessionEnding Exception" + ex.StackTrace.ToString());

            }
        }


        public static void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock)
            {
                los("LogoutUser SessionSwitchReason. SessionLock");

            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock)
            {
                los("LogoutUser SessionSwitchReason. SessionUnlock");

            }
        }

        public static void los(string name2)
        {
            var paths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var subFolderPath = System.IO.Path.Combine(paths, "Workshiftly Client");
            var subFolder = System.IO.Path.Combine(subFolderPath, "MachineSessionDetails.txt");

            var names = DateTime.Now.ToString() + "----" + name2;
            string[] line = new string[] { names };

            File.AppendAllLines(subFolder, line);

        }

        #endregion

        private void MainWindow_Closings(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // if(e.Clo)
        }

        public async void logout()
        {
            try
            {
                ///e.CloseReason
                Int32 companyStartShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift);
                Int32 companyEndShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.endShift);
                var actionTimeStamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

                CommonUtility.HasLogging = false;
                var nio = IsLogeActiveUser();
                var ni = IsLogeActiveUser() && (CommonUtility.Status == null || CommonUtility.Status == "START");
                if (IsLogeActiveUser() && (CommonUtility.Status == null || CommonUtility.Status == "START") && !CommonUtility.CompanyConfigurations.isTrackWithinShift)
                {
                    //var actionTimeStamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                        WorkStatusLog workStatusLog = new WorkStatusLog();

                        workStatusLog.userId = CommonUtility.UserSessions.id;
                        workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                        workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                        workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                        workStatusLog.date = CommonUtility.StartOfDay();
                        workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        workStatusLog.workStatus = "BREAK";
                        workStatusLog.isBreakAutomaticallyStart = false;
                        workStatusLog.breakAutomaticallyStartDuration = 0;
                        workStatusLog.isSynced = false;

                        //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                        //await _clientDbContext.SaveChangesAsync();
                        CommonUtility.Status = "BREAK";

                        WorkStatusLogsList.Add(workStatusLog);
                        HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                        CommonUtility.LogWriteLines("Success", "MainWindow Logout", "workStatus = BREAK " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");
                    }
                }

                if (IsLogeActiveUser() && (CommonUtility.Status == null || CommonUtility.Status == "START")
                    && CommonUtility.CompanyConfigurations.isTrackWithinShift)
                {

                    if ((companyStartShift <= actionTimeStamp) && (companyEndShift >= actionTimeStamp)) // no needed this validation ,only for check
                    {
                        using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                        {
                            List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                            WorkStatusLog workStatusLog = new WorkStatusLog();

                            workStatusLog.userId = CommonUtility.UserSessions.id;
                            workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                            workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                            workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                            workStatusLog.date = CommonUtility.StartOfDay();
                            workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                            workStatusLog.workStatus = "BREAK";
                            workStatusLog.isBreakAutomaticallyStart = false;
                            workStatusLog.breakAutomaticallyStartDuration = 0;
                            workStatusLog.isSynced = false;
                            WorkStatusLogsList.Add(workStatusLog);

                            //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                            //await _clientDbContext.SaveChangesAsync();
                            CommonUtility.Status = "BREAK";
                            HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                            CommonUtility.LogWriteLines("Success", "MainWindow Logout", "workStatus = BREAK " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");

                        }
                    }
                    /////TODO - Check validate

                }

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "MainWindow.xaml Logout", " Java Client Exit Checking Start", "*********************");

            }
        }

        #region method

        private Int32 istrackWithinshift(Int32 actionTime, Int32 startShiftTime)
        {
            if (CommonUtility.CompanyConfigurations.isTrackWithinShift)
            {
                return (startShiftTime <= actionTime) ? actionTime : startShiftTime;
            }
            else
            {
                return actionTime;
            }
        }

        private Int32 istrackWithOutshift(Int32 actionTime, Int32 endShiftTime)
        {
            if (CommonUtility.CompanyConfigurations.isTrackWithinShift)
            {
                return (endShiftTime >= actionTime) ? actionTime : endShiftTime;
            }
            else
            {
                return actionTime;
            }
        }

        public async void StatusUpdate()
        {
            try
            {
                RecoveryStatusLog();
                //  WorkStatusRecovery();

                if (IsLogeActiveUser() && (CommonUtility.Status == null || CommonUtility.Status == "BREAK"))
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        var actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                        WorkStatusLog workStatusLog = new WorkStatusLog();
                        workStatusLog.userId = CommonUtility.UserSessions.id;
                        workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                        workStatusLog.userDate = DateTime.Now.ToString("yyy-MM-dd");
                        workStatusLog.date = CommonUtility.StartOfDay();
                        workStatusLog.actionTimestamp = istrackWithinshift(actionTimestamp, CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift)); // CommonUtility.CompanyConfigurations.isTrackWithinShift && 
                        workStatusLog.workStatus = "START";
                        workStatusLog.isBreakAutomaticallyStart = false;
                        workStatusLog.breakAutomaticallyStartDuration = 0;
                        workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                        workStatusLog.isSynced = false;
                        WorkStatusLogsList.Add(workStatusLog);


                        //_ = await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                        //await _clientDbContext.SaveChangesAsync();
                        CommonUtility.Status = "START";

                        HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                        //  CommonUtility.LogWriteLine("///////////////////////////*************   START -------> workStatus = START   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));
                        CommonUtility.LogWriteLines("Success", "MainWindow StatusUpdate", "workStatus = START " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");

                    }
                }
                else
                {
                    if (IsLogeActiveUser() && CommonUtility.Status == "START")
                    {
                        using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                        {
                            List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                            WorkStatusLog workStatusLog = new WorkStatusLog();

                            workStatusLog.userId = CommonUtility.UserSessions.id;
                            workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                            workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                            workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                            workStatusLog.date = CommonUtility.StartOfDay();
                            workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                            workStatusLog.workStatus = "BREAK";
                            workStatusLog.isBreakAutomaticallyStart = false;
                            workStatusLog.breakAutomaticallyStartDuration = 0;
                            workStatusLog.isSynced = false;
                            WorkStatusLogsList.Add(workStatusLog);

                            //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                            //await _clientDbContext.SaveChangesAsync();
                            CommonUtility.Status = "BREAK";

                            //  HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                            //   CommonUtility.LogWriteLine("///////////////////////////*************   BREAK -------> workStatus = BREAK   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));
                            CommonUtility.LogWriteLines("Success", "MainWindow StatusUpdate", "workStatus = START " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "Main.xaml StatusUpdate", "Status Update Check Error", ex.Message.ToString());

            }


        }

        /// <summary>
        /// copy of StatusUpdate
        /// </summary>
        public async void StatusUpdate2()
        {
            try
            {
                RecoveryStatusLog();
                //  WorkStatusRecovery();

                if (IsLogeActiveUser() && (CommonUtility.Status == null || CommonUtility.Status == "BREAK"))
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        var actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                        WorkStatusLog workStatusLog = new WorkStatusLog();

                        workStatusLog.userId = CommonUtility.UserSessions.id;
                        workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                        workStatusLog.userDate = DateTime.Now.ToString("yyy-MM-dd");
                        workStatusLog.date = CommonUtility.StartOfDay();
                        workStatusLog.actionTimestamp = istrackWithinshift(actionTimestamp, CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift)); // CommonUtility.CompanyConfigurations.isTrackWithinShift && 
                        workStatusLog.workStatus = "START";
                        workStatusLog.isBreakAutomaticallyStart = false;
                        workStatusLog.breakAutomaticallyStartDuration = 0;
                        workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                        workStatusLog.isSynced = false;
                        WorkStatusLogsList.Add(workStatusLog);


                        //_ = await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                        //await _clientDbContext.SaveChangesAsync();
                        CommonUtility.Status = "START";
                        HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                        //  CommonUtility.LogWriteLine("///////////////////////////*************   START -------> workStatus = START   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));
                        CommonUtility.LogWriteLines("Success", "MainWindow StatusUpdate", "workStatus = START " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");

                    }
                }
                else
                {
                    if (IsLogeActiveUser() && CommonUtility.Status == "START")
                    {
                        using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                        {
                            List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                            WorkStatusLog workStatusLog = new WorkStatusLog();

                            workStatusLog.userId = CommonUtility.UserSessions.id;
                            workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                            workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                            workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                            workStatusLog.date = CommonUtility.StartOfDay();
                            workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                            workStatusLog.workStatus = "BREAK";
                            workStatusLog.isBreakAutomaticallyStart = false;
                            workStatusLog.breakAutomaticallyStartDuration = 0;
                            workStatusLog.isSynced = false;
                            WorkStatusLogsList.Add(workStatusLog);

                            //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                            //await _clientDbContext.SaveChangesAsync();
                            CommonUtility.Status = "BREAK";
                            HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                            //   CommonUtility.LogWriteLine("///////////////////////////*************   BREAK -------> workStatus = BREAK   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));
                            CommonUtility.LogWriteLines("Success", "MainWindow StatusUpdate", "workStatus = START " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "Main.xaml StatusUpdate", "Status Update Check Error", ex.Message.ToString());

            }


        }



        /// <summary>
        /// when the application kill or disposed of - log status is saved as a start, therefore, it needs to update last active time as Break 
        /// </summary>
        private async void WorkStatusRecovery()
        {
            try
            {

                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {

                    var workStatusLog = _clientDbContext.WorkStatusLogs.Where(workStatus =>
                                  workStatus.userId == CommonUtility.UserSessions.id && workStatus.companyId == CommonUtility.UserSessions.companyId).ToList().LastOrDefault();

                    var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id).ToList().LastOrDefault();

                    var todayStartTime = CommonUtility.StartOfDay();
                    var todayEndTime = CommonUtility.EndOfDay();
                    if (workStatusLog == null) return;

                    if (workStatusLog.workStatus == "START" && (todayStartTime <= workStatusLog.actionTimestamp) && (workStatusLog.actionTimestamp <= todayEndTime))
                    {


                        if (workStatusLog.actionTimestamp < Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) && workStatusLog.actionTimestamp < activeWindow.endTimestamp)
                        {


                            WorkStatusLog workStatusLogNew = new WorkStatusLog();

                            workStatusLogNew.userId = CommonUtility.UserSessions.id;
                            workStatusLogNew.companyId = CommonUtility.UserSessions.companyId;
                            workStatusLog.deviceId = CommonUtility.DeviceSettings.id;
                            workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                            workStatusLogNew.date = CommonUtility.StartOfDay();
                            workStatusLogNew.actionTimestamp = activeWindow.endTimestamp;
                            workStatusLogNew.workStatus = "BREAK";
                            workStatusLogNew.isBreakAutomaticallyStart = false;
                            workStatusLogNew.breakAutomaticallyStartDuration = 0;
                            workStatusLogNew.isSynced = false;

                            await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                            await _clientDbContext.SaveChangesAsync();

                        }


                    }


                }
            }
            catch (Exception)
            {


            }
        }


        private bool IsLogeActiveUser()
        {
            var ni = CommonUtility.UserSessions?.id != null && CommonUtility.UserSessions?.companyId != null;
            return ni;
        }


        /// <summary>
        /// create LogAppDAta
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns>LogaAppData object</returns>
        public LogAppData insert(string type, string data)
        {
            return new LogAppData { LogType = type, LogData = data };
        }


        private async void Window_Loaded()
        {
            //creates a DispatchTimer object.
            CommonUtility.IdledispatcherTimer = new DispatcherTimer();

            // sets the Tick and Interval of a DispatchTimer.
            CommonUtility.IdledispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            CommonUtility.IdledispatcherTimer.Tick += new EventHandler(dispatcherWithShift_Tick);
            CommonUtility.IdledispatcherTimer.Interval = new TimeSpan(0, 0, 1);


            //The Start method is used to start a DispatchTimer.
            CommonUtility.IdledispatcherTimer.Start();

        }

        private void dispatcherWithShift_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {

                    //var activeuser = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                    //if (IsLogeActiveUser() && !first)
                    //{
                    //    if (CommonUtility.socketIOAdapter == null)
                    //    {
                    //        CommonUtility.socketIOAdapter = new SocketIOAdapter();
                    //        first = true;
                    //    }
                    //    else
                    //    {
                    //        CommonUtility.socketIOAdapter.ReconnectAsyncSockdtIo();
                    //        first = true;
                    //    }

                    //}

                }
                catch (Exception)
                {

                    // throw;
                }

            }));
        }

        #endregion

        #region Idle

        /// <summary>
        /// Idle Time clocker 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                //1 min
                try
                {
                    CommonUtility.TickCountIdleTime = Win32KeyMouseAPICall.GetIdleTime();
                    CommonUtility.currentIdleTime = Win32KeyMouseAPICall.GetLastInputTimesUnix();
                    var newIdle = CommonUtility.CompanyConfigurations.appIdleTime != 0 ? CommonUtility.CompanyConfigurations.appIdleTime : 3;
                    var idleTime = newIdle * 60 * 1000;
                    //var idleTime = 1 * 60 * 1000;
                    //60000
                    var ni = Win32KeyMouseAPICall.GetIdleTime();
                    var oi = idleTime;
                    if (Win32KeyMouseAPICall.GetIdleTime() > idleTime && IsLogeActiveUser())
                    {
                        Int32 EndTimeStamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        CommonUtility.ActiveWindoDispatcherTimer.Stop();
                        CommonUtility.ScreenCaptureUtility.Stop();
                        ActiveWindowUtility.saveActiveWindowStopIdle(EndTimeStamp);
                        if (!CommonUtility.IsIdleDbWrite)
                        {
                            using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                            {
                                List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                                WorkStatusLog workStatusLog = new WorkStatusLog();

                                workStatusLog.userId = CommonUtility.UserSessions.id;
                                workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                                workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                                workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                                workStatusLog.date = CommonUtility.StartOfDay();
                                workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                                workStatusLog.workStatus = "BREAK";
                                workStatusLog.isBreakAutomaticallyStart = false;
                                workStatusLog.breakAutomaticallyStartDuration = 0;
                                workStatusLog.isSynced = false;
                                WorkStatusLogsList.Add(workStatusLog);

                                //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                                //await _clientDbContext.SaveChangesAsync();
                                CommonUtility.Status = "BREAK";

                                HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                            }

                            CommonUtility.LogWriteLines("Success", "MainWindow IdledispatcherTimer", "Idle - start -------> workStatus = BREAK  " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");

                            CommonUtility.IsIdleDbWrite = true;

                            //if (CommonUtility.IsIdleListener)
                            //{
                            IdleTimeListener.activeSlientTrackingHandler(CommonUtility.currentIdleTime);
                            CommonUtility.IsIdleListener = true;
                            //  }
                        }
                        //else if (!CommonUtility.IsIdleListener)
                        //{
                        //    IdleTimeListener.activeSlientTrackingHandler(CommonUtility.currentIdleTime);
                        //    CommonUtility.IsIdleListener = true;
                        //}
                        else
                        {

                        }

                    }
                    else
                    {


                    }

                }
                catch (Exception ex)
                {
                    CommonUtility.LogWriteLines("Error", "Main.xaml dispatcherTimer_Tick Idle ", "IDLE Error", ex.Message.ToString());

                }


            }));

            // CommandManager.InvalidateRequerySuggested();

        }

        #endregion

        #region Logging

        /// <summary>
        /// read xml file
        /// </summary>
        private async void AutoRunWindowsLoggin()
        {
            try
            {

                XmlDocument Xdoc = new XmlDocument();

                Xdoc.Load(XMLPath);

                XmlNodeList nodeList = Xdoc.DocumentElement.SelectNodes("/Authontication");
                //loop through each node and save it value in NodeStr
                var AUTH_TOKEN = "";
                var USER_SESSION_VALUE = new object();
                var COMPANY_SETTINGS = new object();
                var USER_COMPANY_INSTANCE = new object();
                var COMPANY_CONFIGURATION = new object();
                var DEVICE_SETTINGS = new object();
                var LOGGED_IN_DEVICE_SETTINGS = new object();

                var USER_SESSION = "";
                var COMPANY_SETTINGS_Node = "";
                var USER_COMPANY_INSTANCE_Node = "";
                var COMPANY_CONFIGURATION_Node = "";
                var DEVICE_SETTINGS_Node = "";
                var LOGGED_IN_DEVICE_SETTINGS_Node = "";


                List<LogAppData> list = new List<LogAppData>();

                foreach (XmlNode node in nodeList)
                {

                    AUTH_TOKEN = node.SelectSingleNode("AUTH_TOKEN").InnerText;
                    list.Add(insert("AUTH_TOKEN", AUTH_TOKEN));
                    UserSession.AuthToken = AUTH_TOKEN;

                    USER_SESSION = node.SelectSingleNode("USER_SESSION").InnerText;
                    USER_SESSION_VALUE = JsonConvert.DeserializeObject(USER_SESSION);
                    list.Add(insert("USER_SESSION", USER_SESSION));

                    COMPANY_SETTINGS_Node = node.SelectSingleNode("USER_SESSION").InnerText;
                    COMPANY_SETTINGS = JsonConvert.DeserializeObject(COMPANY_SETTINGS_Node);
                    list.Add(insert("COMPANY_SETTINGS", COMPANY_SETTINGS_Node));

                    USER_COMPANY_INSTANCE_Node = node.SelectSingleNode("USER_COMPANY_INSTANCE").InnerText;
                    USER_COMPANY_INSTANCE = JsonConvert.DeserializeObject(USER_COMPANY_INSTANCE_Node);
                    list.Add(insert("USER_COMPANY_INSTANCE", USER_COMPANY_INSTANCE_Node));

                    COMPANY_CONFIGURATION_Node = node.SelectSingleNode("COMPANY_CONFIGURATION").InnerText;
                    COMPANY_CONFIGURATION = JsonConvert.DeserializeObject(COMPANY_CONFIGURATION_Node);
                    list.Add(insert("COMPANY_CONFIGURATION", COMPANY_CONFIGURATION_Node));

                    DEVICE_SETTINGS_Node = node.SelectSingleNode("DEVICE_SETTINGS").InnerText;
                    DEVICE_SETTINGS = JsonConvert.DeserializeObject(DEVICE_SETTINGS_Node);
                    list.Add(insert("DEVICE_SETTINGS", DEVICE_SETTINGS_Node));

                    LOGGED_IN_DEVICE_SETTINGS_Node = node.SelectSingleNode("LOGGED_IN_DEVICE_SETTINGS").InnerText;
                    LOGGED_IN_DEVICE_SETTINGS = JsonConvert.DeserializeObject(LOGGED_IN_DEVICE_SETTINGS_Node);
                    list.Add(insert("LOGGED_IN_DEVICE_SETTINGS", LOGGED_IN_DEVICE_SETTINGS_Node));


                };

                try
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {

                        _clientDbContext.LogAppDatas.RemoveRange(_clientDbContext.LogAppDatas);
                        _clientDbContext.LogAppDatas.AddRange(list);

                        await _clientDbContext.SaveChangesAsync();
                        CommonUtility.HasLogging = true;

                    }
                    CommonUtility.HasLogging = true;
                    StoredOnState(list);
                    StatusUpdate();
                    CommonUtility.screenCaptureUtilityFunction = new ScreenshotUtility();

                    if (CommonUtility.HasLogging)
                        File.Delete(XMLPath);
                }
                catch (Exception ex)
                {
                    CommonUtility.LogWriteLines("Catch", "MainWindow AutoRunWindowsLoggin", "Db write error  ", ex.Message.ToString());

                }

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Catch", "MainWindow AutoRunWindowsLoggin", "Xml Read Error  ", ex.Message.ToString());
            }

        }


        private async void LogingCredentialsUpdate()
        {
            try
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {

                    var count = _clientDbContext.LogAppDatas.Where(log => log.LogType != null).ToList().Count;
                    var logAppData = _clientDbContext.LogAppDatas.Where(log => log.LogType != null).ToList();
                    if (count >= 6)
                    {
                        CommonUtility.HasLogging = true;
                        StoredOnState(logAppData);
                        StatusUpdate();

                    }
                    else
                    {
                        CommonUtility.HasLogging = false;
                    }

                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "Main.xaml LogingCredentialsUpdate", "Loging Credential Check Error", ex.Message.ToString());

            }

        }

        /// <summary>
        /// use to store data
        /// </summary>
        /// <param name="logs">Loging data</param>
        /// 
        private async void StoredOnState(List<LogAppData> logs)
        {
            try
            {
                foreach (var log in logs)
                {
                    LogType tryParseResultlogType;
                    if (Enum.TryParse<LogType>(log.LogType, out tryParseResultlogType))
                    {
                        switch (tryParseResultlogType)
                        {
                            case LogType.AUTH_TOKEN:
                                //
                                break;
                            case LogType.USER_SESSION:
                                UserSessions userSession = StoreOnSate<UserSessions>(log.LogData);
                                if (userSession != null)
                                {
                                    CommonUtility.UserSessions = userSession;
                                    var activeuser = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                                    //if (IsLogeActiveUser() && !first)
                                    //{
                                    //    CommonUtility.socketIOAdapter = new SocketIOAdapter();
                                    //    first = true;

                                    //}
                                }
                                break;
                            case LogType.COMPANY_SETTINGS:
                                var companySettings = StoreOnSate<CompanySettings>(log.LogData);
                                if (companySettings != null)
                                {
                                    CommonUtility.CompanySettings = companySettings;
                                }
                                break;
                            case LogType.USER_COMPANY_INSTANCE:
                                var userCompanyInstance = StoreOnSate<Company>(log.LogData);
                                if (userCompanyInstance != null)
                                {
                                    CommonUtility.Company = userCompanyInstance;
                                }
                                break;
                            case LogType.COMPANY_CONFIGURATION:
                                var companyConfiguration = StoreOnSate<CompanyConfiguration>(log.LogData);
                                if (companyConfiguration != null)
                                {
                                    CommonUtility.CompanyConfigurations = companyConfiguration;
                                }
                                break;
                            case LogType.DEVICE_SETTINGS:
                                var deviceSetting = StoreOnSate<DeviceConfiguration>(log.LogData);
                                if (deviceSetting != null)
                                {
                                    CommonUtility.DeviceSettings = deviceSetting;
                                    try
                                    {
                                        //using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                                        //{
                                        //    _clientDbContext.DeviceSettings.RemoveRange(_clientDbContext.DeviceSettings);
                                        //    await _clientDbContext.DeviceSettings.AddAsync(deviceSetting);
                                        //    await _clientDbContext.SaveChangesAsync();
                                        //}
                                    }
                                    catch (Exception exe)
                                    {
                                        CommonUtility.LogWriteLines("Catch", "MainWindow StoredOnState", "device setting save to db Error  ", exe.Message.ToString());
                                    }

                                }
                                break;
                            case LogType.LOGGED_IN_DEVICE_SETTINGS:
                                var logdeviceSetting = StoreOnSate<DeviceSetting>(log.LogData);
                                if (logdeviceSetting != null)
                                {
                                    CommonUtility.LoggedDeviceSettings = logdeviceSetting;


                                }
                                break;
                            default:
                                break;
                        }
                    }





                }

                if (CommonUtility.socketIOAdapter == null)
                {
                    CommonUtility.socketIOAdapter = new SocketIOAdapter();
                    first = true;
                }
                else
                {
                    CommonUtility.socketIOAdapter.ReconnectAsyncSockdtIo();
                    first = true;
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Catch", "MainWindow StoredOnState", "Xml Read and mapping Error  ", ex.Message.ToString());
            }

        }



        /// <summary>
        /// this usign genarics
        /// </summary>
        /// <typeparam name="T"> Entity Class - Class type is Dynamically Change</typeparam>
        /// <param name="LogData">T type of class object</param>
        /// <returns></returns>
        private T StoreOnSate<T>(string LogData) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(LogData);
                //T returnData = JsonConvert.DeserializeObject<T>(LogData);
            }
            catch (Exception ex)
            {
                return new T();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shift"> shift time stamp(start or end)</param>
        /// <param name="currentTime"> action fired timestamp</param>
        /// <returns> retun true when time gap lessthan 6</returns>
        private bool timeGapNotMorethan6(Int32 shift, Int32 currentTime)
        {
            var diffent = currentTime - shift;
            if (diffent <= 6)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region startup
        private void ActiveWindowUtilitys()
        {
            try
            {
                //  var activeWindowUtitliy = new ActiveWindowUtility();
                CommonUtility.ActiveWindowUtilityFunction = new ActiveWindowUtility();

                CommonUtility.ActiveWindoDispatcherTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(3)
                };

                CommonUtility.ActiveWindoDispatcherTimer.Tick += async delegate (object sender, EventArgs e)
                {
                    Int32 actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                    Int32 companyStartShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift);
                    Int32 companyEndShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.endShift);

                    if (IsLogeActiveUser() && CommonUtility.CompanyConfigurations.isTrackWithinShift
                    )
                    {
                        if ((companyStartShift <= actionTimestamp) && (companyEndShift >= actionTimestamp))
                        {
                            CommonUtility.ActiveWindowUtilityFunction.activeWindow();
                        }
                        if (CommonUtility.ActiveWindoDispatcherTimer.IsEnabled && (companyEndShift <= actionTimestamp) && timeGapNotMorethan6(companyEndShift, actionTimestamp))
                        {
                            ActiveWindowUtility.saveActiveWindowStopIdle(companyEndShift);
                        }


                    }
                    if (IsLogeActiveUser() && !CommonUtility.CompanyConfigurations.isTrackWithinShift)
                    {
                        CommonUtility.ActiveWindowUtilityFunction.activeWindow();
                    }
                };

                CommonUtility.ActiveWindoDispatcherTimer.Start();
            }
            catch (Exception ex)
            {

            }
        }


        private void ScreenCaptureUtility()
        {
            try
            {
                CommonUtility.screenCaptureUtilityFunction = new ScreenshotUtility();

                CommonUtility.ScreenCaptureUtility = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };

                CommonUtility.ScreenCaptureUtility.Tick += async delegate (object sender, EventArgs e)
                {


                    Int32 actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                    Int32 companyStartShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift);
                    Int32 companyEndShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.endShift);

                    if (IsLogeActiveUser())
                    {
                        if (CommonUtility.CompanyConfigurations.isTrackWithinShift
                            && (companyStartShift <= actionTimestamp)
                            && (companyEndShift >= actionTimestamp))
                        {
                            CommonUtility.screenCaptureUtilityFunction.getRunUtilityRunnable();
                        }
                        else if (!CommonUtility.CompanyConfigurations.isTrackWithinShift)
                        {
                            CommonUtility.screenCaptureUtilityFunction.getRunUtilityRunnable();
                        }

                    }
                    // HttpRequestCaller.syncActivityWindows();
                };

                CommonUtility.ScreenCaptureUtility.Start();
            }
            catch (Exception ex)
            {

            }



        }

        private void SyncAtivityWindows()
        {
            try
            {
                //CommonUtility.syncActiveWindow.Start();
                var _activeTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMinutes(5)
                    // Interval = TimeSpan.FromSeconds(20)
                };
                _activeTimer.Tick += async delegate (object sender, EventArgs e)
                {

                    Thread syncActiveWindow = new Thread(() => HttpRequestCaller.syncActivityWindows());
                    syncActiveWindow.Start();

                    Thread callPostSyncWorkStatusLogs = new Thread(() => HttpRequestCaller.CallPostSyncWorkStatusLogs());
                    callPostSyncWorkStatusLogs.Start();

                    Thread capture = new Thread(() => HttpRequestCaller.syncScreenshots());
                    capture.Start();

                };
                _activeTimer.Start();
            }
            catch (Exception ex)
            {

            }


        }
        #endregion


        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ProcessProtection.Unprotect();
                CommonUtility.LogWriteLine("///////////////////////////*************  Window_Closing -------> killinggggggggggggggggggggggggg   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

                if (IsLogeActiveUser() && CommonUtility.Status == "START")
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                        WorkStatusLog workStatusLog = new WorkStatusLog();

                        workStatusLog.userId = CommonUtility.UserSessions.id;
                        workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                        workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                        workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                        workStatusLog.date = CommonUtility.StartOfDay();
                        workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        workStatusLog.workStatus = "BREAK";
                        workStatusLog.isBreakAutomaticallyStart = false;
                        workStatusLog.breakAutomaticallyStartDuration = 0;
                        workStatusLog.isSynced = false;
                        WorkStatusLogsList.Add(workStatusLog);

                        //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                        //await _clientDbContext.SaveChangesAsync();
                        CommonUtility.Status = "BREAK";
                        HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                        CommonUtility.LogWriteLine("///////////////////////////*************   BREAK -------> workStatus = BREAK   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

                    }
                }


                CommonUtility.LogWriteLine("///////////////////////////*************  Window_Closing -------> workStatus = BREAK   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

            }
            catch (Exception ex)
            {

            }
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                ///e.CloseReason
                ///
                // ProcessProtection.Unprotect();
                if (IsLogeActiveUser() && CommonUtility.Status == "START")
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                        WorkStatusLog workStatusLog = new WorkStatusLog();

                        workStatusLog.userId = CommonUtility.UserSessions.id;
                        workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                        workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                        workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                        workStatusLog.date = CommonUtility.StartOfDay();
                        workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        workStatusLog.workStatus = "BREAK";
                        workStatusLog.isBreakAutomaticallyStart = false;
                        workStatusLog.breakAutomaticallyStartDuration = 0;
                        workStatusLog.isSynced = false;
                        WorkStatusLogsList.Add(workStatusLog);

                        //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                        //await _clientDbContext.SaveChangesAsync();
                        CommonUtility.Status = "BREAK";
                        HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                        CommonUtility.LogWriteLine("///////////////////////////*************   BREAK -------> workStatus = BREAK   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

                    }
                }


                CommonUtility.LogWriteLine("///////////////////////////*************  Window_Closed -------> workStatus = BREAK   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

            }
            catch (Exception ex)
            {

            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SystemEvents.SessionEnding += new Microsoft.Win32.SessionEndingEventHandler(SystemEvents_SessionEnding);
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(OnPowerModeChanged);
            this.Hide();

        }


        public static void OnPowerModeChangeds(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    CommonUtility.LogWriteLines("Success", "MAIN.xaml OnPowerModeChanged", "PowerMode: OS is resuming from suspended state", "-----------");
                    // MessageBox.Show("PowerMode: OS is resuming from suspended state");
                    break;

                case PowerModes.Suspend:
                    CommonUtility.LogWriteLines("Success", "MAIN.xaml OnPowerModeChanged", "PowerMode: OS is about to be suspended", "-----------");
                    //  MessageBox.Show("PowerMode: OS is about to be suspended");
                    break;
            }
        }

        private async void RecoveryStatusLog()
        {
            try
            {

                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {

                    var workStatusLog = _clientDbContext.WorkStatusLogs.Where(workStatus =>
                                  workStatus.userId == CommonUtility.UserSessions.id && workStatus.companyId == CommonUtility.UserSessions.companyId).ToList().LastOrDefault();

                    Int32 companyStartShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.startShift);
                    Int32 companyEndShift = CommonUtility.CompanyConfigurShiftTimes(CommonUtility.Company.endShift);
                    var actionTimeStamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());


                    if (IsLogeActiveUser() && (CommonUtility.Status == null || CommonUtility.Status == "START") && !CommonUtility.CompanyConfigurations.isTrackWithinShift)
                    {
                        if (workStatusLog != null && workStatusLog.workStatus == "START")
                        {
                            //// last active window day
                            var workStatusLogdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(workStatusLog.actionTimestamp);
                            var workStatusLogdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(workStatusLog.actionTimestamp);



                            var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id && (workStatusLogdayStartTime <= active.startedTimestamp && active.endTimestamp <= workStatusLogdayEndTime)).ToList().LastOrDefault();
                            //  var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id).ToList().LastOrDefault();

                            if (activeWindow == null && workStatusLog == null) return;
                            //today 
                            var todayStartTime = CommonUtility.StartOfDay();
                            var todayEndTime = CommonUtility.EndOfDay();

                            //// last active windwow day
                            var activeWindowdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(activeWindow.endTimestamp);
                            var activeWindowdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(activeWindow.endTimestamp);


                            if (workStatusLog == null) return;

                            if (workStatusLog.workStatus == "START" && (workStatusLogdayStartTime <= workStatusLog.actionTimestamp) && (workStatusLog.actionTimestamp <= workStatusLogdayEndTime))
                            {


                                if (workStatusLog.actionTimestamp < Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) && workStatusLog.actionTimestamp < activeWindow.endTimestamp)
                                {


                                    List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                                    WorkStatusLog workStatusLogNew = new WorkStatusLog();

                                    workStatusLogNew.userId = workStatusLog.userId;
                                    workStatusLogNew.companyId = workStatusLog.companyId;
                                    workStatusLog.deviceId = workStatusLog.deviceId;
                                    workStatusLogNew.userDate = CommonUtility.UnixTimeStampToDateTime(activeWindow.endTimestamp).ToString("yyyy-MM-dd");
                                    //   workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                                    workStatusLogNew.date = workStatusLogdayStartTime;
                                    workStatusLogNew.actionTimestamp = activeWindow.endTimestamp;
                                    workStatusLogNew.workStatus = "BREAK";
                                    workStatusLogNew.isBreakAutomaticallyStart = false;
                                    workStatusLogNew.breakAutomaticallyStartDuration = 0;
                                    workStatusLogNew.isSynced = false;
                                    WorkStatusLogsList.Add(workStatusLogNew);

                                    //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                                    //await _clientDbContext.SaveChangesAsync();
                                    HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                                }
                                else if (workStatusLog.actionTimestamp >= activeWindow.endTimestamp)
                                {
                                    List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                                    WorkStatusLog workStatusLogNew = new WorkStatusLog();

                                    workStatusLogNew.userId = workStatusLog.userId;
                                    workStatusLogNew.companyId = workStatusLog.companyId;
                                    workStatusLog.deviceId = workStatusLog.deviceId;
                                    workStatusLogNew.userDate = CommonUtility.UnixTimeStampToDateTime(workStatusLog.actionTimestamp).ToString("yyyy-MM-dd");
                                    //  workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                                    workStatusLogNew.date = workStatusLogdayStartTime;
                                    workStatusLogNew.actionTimestamp = workStatusLog.actionTimestamp;
                                    workStatusLogNew.workStatus = "BREAK";
                                    workStatusLogNew.isBreakAutomaticallyStart = false;
                                    workStatusLogNew.breakAutomaticallyStartDuration = 0;
                                    workStatusLogNew.isSynced = false;
                                    WorkStatusLogsList.Add(workStatusLogNew);

                                    //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                                    //await _clientDbContext.SaveChangesAsync();
                                    HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                                }


                            }
                        }
                    }
                    if (IsLogeActiveUser() && (CommonUtility.Status == null || CommonUtility.Status == "START")
                    && CommonUtility.CompanyConfigurations.isTrackWithinShift)
                    {

                        if ((companyStartShift <= actionTimeStamp) && (companyEndShift >= actionTimeStamp)) // no needed this validation ,only for check
                        {

                            List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                            WorkStatusLog workStatusLogNew = new WorkStatusLog();

                            workStatusLogNew.userId = CommonUtility.UserSessions.id;
                            workStatusLogNew.companyId = CommonUtility.UserSessions.companyId;
                            workStatusLogNew.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                            workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                            workStatusLogNew.date = CommonUtility.StartOfDay();
                            workStatusLogNew.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                            workStatusLogNew.workStatus = "BREAK";
                            workStatusLogNew.isBreakAutomaticallyStart = false;
                            workStatusLogNew.breakAutomaticallyStartDuration = 0;
                            workStatusLogNew.isSynced = false;
                            WorkStatusLogsList.Add(workStatusLogNew);

                            //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                            //await _clientDbContext.SaveChangesAsync();
                            CommonUtility.Status = "BREAK";
                            HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                            CommonUtility.LogWriteLines("Success", "MainWindow Logout", "workStatus = BREAK " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");


                        }
                        /////TODO - Check validate

                    }



                    if (workStatusLog != null && workStatusLog.workStatus == "START")
                    {
                        //// last active window day
                        var workStatusLogdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(workStatusLog.actionTimestamp);
                        var workStatusLogdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(workStatusLog.actionTimestamp);



                        var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id && (workStatusLogdayStartTime <= active.startedTimestamp && active.endTimestamp <= workStatusLogdayEndTime)).ToList().LastOrDefault();
                        //  var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id).ToList().LastOrDefault();

                        if (activeWindow == null && workStatusLog == null) return;
                        //today 
                        var todayStartTime = CommonUtility.StartOfDay();
                        var todayEndTime = CommonUtility.EndOfDay();

                        //// last active windwow day
                        var activeWindowdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(activeWindow.endTimestamp);
                        var activeWindowdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(activeWindow.endTimestamp);


                        if (workStatusLog == null) return;

                        if (workStatusLog.workStatus == "START" && (workStatusLogdayStartTime <= workStatusLog.actionTimestamp) && (workStatusLog.actionTimestamp <= workStatusLogdayEndTime))
                        {


                            if (workStatusLog.actionTimestamp < Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) && workStatusLog.actionTimestamp < activeWindow.endTimestamp)
                            {


                                List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                                WorkStatusLog workStatusLogNew = new WorkStatusLog();

                                workStatusLogNew.userId = workStatusLog.userId;
                                workStatusLogNew.companyId = workStatusLog.companyId;
                                workStatusLog.deviceId = workStatusLog.deviceId;
                                workStatusLogNew.userDate = CommonUtility.UnixTimeStampToDateTime(activeWindow.endTimestamp).ToString("yyyy-MM-dd");
                                // workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                                workStatusLogNew.date = workStatusLogdayStartTime;
                                workStatusLogNew.actionTimestamp = activeWindow.endTimestamp;
                                workStatusLogNew.workStatus = "BREAK";
                                workStatusLogNew.isBreakAutomaticallyStart = false;
                                workStatusLogNew.breakAutomaticallyStartDuration = 0;
                                workStatusLogNew.isSynced = false;
                                WorkStatusLogsList.Add(workStatusLogNew);

                                //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                                //await _clientDbContext.SaveChangesAsync();
                                HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);

                            }
                            else if (workStatusLog.actionTimestamp >= activeWindow.endTimestamp)
                            {
                                List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                                WorkStatusLog workStatusLogNew = new WorkStatusLog();

                                workStatusLogNew.userId = workStatusLog.userId;
                                workStatusLogNew.companyId = workStatusLog.companyId;
                                workStatusLog.deviceId = workStatusLog.deviceId;
                                workStatusLogNew.userDate = CommonUtility.UnixTimeStampToDateTime(workStatusLog.actionTimestamp).ToString("yyyy-MM-dd");
                                //  workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                                workStatusLogNew.date = workStatusLogdayStartTime;
                                workStatusLogNew.actionTimestamp = workStatusLog.actionTimestamp;
                                workStatusLogNew.workStatus = "BREAK";
                                workStatusLogNew.isBreakAutomaticallyStart = false;
                                workStatusLogNew.breakAutomaticallyStartDuration = 0;
                                workStatusLogNew.isSynced = false;
                                WorkStatusLogsList.Add(workStatusLogNew);

                                //await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                                //await _clientDbContext.SaveChangesAsync();
                                HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                            }


                        }
                    }



                }
            }
            catch (Exception)
            {


            }
        }

        private async void OldUserStatusRecovery()
        {

        }
        /// <summary>
        /// copy of recoveryStatusLog
        /// </summary>
        private async void RecoveryStatusLog3()
        {
            try
            {

                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {

                    var workStatusLog = _clientDbContext.WorkStatusLogs.Where(workStatus =>
                                  workStatus.userId == CommonUtility.UserSessions.id && workStatus.companyId == CommonUtility.UserSessions.companyId).ToList().LastOrDefault();

                    if (workStatusLog != null && workStatusLog.workStatus == "START")
                    {
                        //// last active window day
                        var workStatusLogdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(workStatusLog.actionTimestamp);
                        var workStatusLogdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(workStatusLog.actionTimestamp);



                        var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id && (workStatusLogdayStartTime <= active.startedTimestamp && active.endTimestamp <= workStatusLogdayEndTime)).ToList().LastOrDefault();
                        //  var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id).ToList().LastOrDefault();

                        if (activeWindow == null && workStatusLog == null) return;
                        //today 
                        var todayStartTime = CommonUtility.StartOfDay();
                        var todayEndTime = CommonUtility.EndOfDay();

                        //// last active windwow day
                        var activeWindowdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(activeWindow.endTimestamp);
                        var activeWindowdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(activeWindow.endTimestamp);


                        if (workStatusLog == null) return;

                        if (workStatusLog.workStatus == "START" && (workStatusLogdayStartTime <= workStatusLog.actionTimestamp) && (workStatusLog.actionTimestamp <= workStatusLogdayEndTime))
                        {


                            if (workStatusLog.actionTimestamp < Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) && workStatusLog.actionTimestamp < activeWindow.endTimestamp)
                            {


                                WorkStatusLog workStatusLogNew = new WorkStatusLog();

                                workStatusLogNew.userId = workStatusLog.userId;
                                workStatusLogNew.companyId = workStatusLog.companyId;
                                workStatusLog.deviceId = workStatusLog.deviceId;
                                workStatusLogNew.userDate = CommonUtility.UnixTimeStampToDateTime(activeWindow.endTimestamp).ToString("yyyy-MM-dd");
                                //   workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                                workStatusLogNew.date = workStatusLogdayStartTime;
                                workStatusLogNew.actionTimestamp = activeWindow.endTimestamp;
                                workStatusLogNew.workStatus = "BREAK";
                                workStatusLogNew.isBreakAutomaticallyStart = false;
                                workStatusLogNew.breakAutomaticallyStartDuration = 0;
                                workStatusLogNew.isSynced = false;

                                await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                                await _clientDbContext.SaveChangesAsync();

                            }
                            else if (workStatusLog.actionTimestamp >= activeWindow.endTimestamp)
                            {
                                WorkStatusLog workStatusLogNew = new WorkStatusLog();

                                workStatusLogNew.userId = workStatusLog.userId;
                                workStatusLogNew.companyId = workStatusLog.companyId;
                                workStatusLog.deviceId = workStatusLog.deviceId;
                                workStatusLogNew.userDate = CommonUtility.UnixTimeStampToDateTime(workStatusLog.actionTimestamp).ToString("yyyy-MM-dd");
                                //  workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                                workStatusLogNew.date = workStatusLogdayStartTime;
                                workStatusLogNew.actionTimestamp = workStatusLog.actionTimestamp;
                                workStatusLogNew.workStatus = "BREAK";
                                workStatusLogNew.isBreakAutomaticallyStart = false;
                                workStatusLogNew.breakAutomaticallyStartDuration = 0;
                                workStatusLogNew.isSynced = false;

                                await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                                await _clientDbContext.SaveChangesAsync();
                            }


                        }
                    }



                }
            }
            catch (Exception)
            {


            }
        }

        private async void RecoveryStatusLog2()
        {
            try
            {

                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {

                    var workStatusLog = _clientDbContext.WorkStatusLogs.Where(workStatus =>
                                  workStatus.userId == CommonUtility.UserSessions.id && workStatus.companyId == CommonUtility.UserSessions.companyId).ToList().LastOrDefault();

                    //// last active window day
                    var workStatusLogdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(workStatusLog.actionTimestamp);
                    var workStatusLogdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(workStatusLog.actionTimestamp);



                    var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id && (workStatusLogdayStartTime <= active.startedTimestamp && active.endTimestamp <= workStatusLogdayEndTime)).ToList().LastOrDefault();
                    //  var activeWindow = _clientDbContext.ActivietyWindows.Where(active => active.userId == CommonUtility.UserSessions.id).ToList().LastOrDefault();

                    if (activeWindow == null && workStatusLog == null) return;
                    //today 
                    var todayStartTime = CommonUtility.StartOfDay();
                    var todayEndTime = CommonUtility.EndOfDay();

                    //// last active windwow day
                    var activeWindowdayStartTime = CommonUtility.UnixTimeStampToDateTimeStartOfDay(activeWindow.endTimestamp);
                    var activeWindowdayEndTime = CommonUtility.UnixTimeStampToDateTimeEndOfDay(activeWindow.endTimestamp);


                    if (workStatusLog == null) return;

                    if (workStatusLog.workStatus == "START" && (todayStartTime <= workStatusLog.actionTimestamp) && (workStatusLog.actionTimestamp <= todayEndTime))
                    {


                        if (workStatusLog.actionTimestamp < Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) && workStatusLog.actionTimestamp < activeWindow.endTimestamp)
                        {


                            WorkStatusLog workStatusLogNew = new WorkStatusLog();

                            workStatusLogNew.userId = CommonUtility.UserSessions.id;
                            workStatusLogNew.companyId = CommonUtility.UserSessions.companyId;
                            workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                            workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                            workStatusLogNew.date = CommonUtility.StartOfDay();
                            workStatusLogNew.actionTimestamp = activeWindow.endTimestamp;
                            workStatusLogNew.workStatus = "BREAK";
                            workStatusLogNew.isBreakAutomaticallyStart = false;
                            workStatusLogNew.breakAutomaticallyStartDuration = 0;
                            workStatusLogNew.isSynced = false;

                            await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                            await _clientDbContext.SaveChangesAsync();

                        }


                    }
                    else if (workStatusLog.workStatus == "START" && (todayStartTime <= workStatusLog.actionTimestamp) && (workStatusLog.actionTimestamp <= todayEndTime))
                    {


                        if (workStatusLog.actionTimestamp < Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) && workStatusLog.actionTimestamp < activeWindow.endTimestamp)
                        {


                            WorkStatusLog workStatusLogNew = new WorkStatusLog();

                            workStatusLogNew.userId = CommonUtility.UserSessions.id;
                            workStatusLogNew.companyId = CommonUtility.UserSessions.companyId;
                            workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                            workStatusLogNew.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                            workStatusLogNew.date = CommonUtility.StartOfDay();
                            workStatusLogNew.actionTimestamp = activeWindow.endTimestamp;
                            workStatusLogNew.workStatus = "BREAK";
                            workStatusLogNew.isBreakAutomaticallyStart = false;
                            workStatusLogNew.breakAutomaticallyStartDuration = 0;
                            workStatusLogNew.isSynced = false;

                            await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLogNew);
                            await _clientDbContext.SaveChangesAsync();

                        }


                    }


                }
            }
            catch (Exception)
            {


            }
        }

        private const string GoogleMapsApiKey = "AIzaSyA5gEYIDrr7UL6StikpWE43CKfRWnB-ATU";
        private const string GoogleTimeZoneApiUrl = "https://maps.googleapis.com/maps/api/timezone/json";


        public LocationInfo GetLocationInfo(double latitude, double longitude)
        {
            string apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={GoogleMapsApiKey}";

            using (WebClient client = new WebClient())
            {
                try
                {
                    string json = client.DownloadString(apiUrl);
                    JObject data = JObject.Parse(json);

                    if (data["status"].ToString() == "OK")
                    {
                        JArray results = (JArray)data["results"];
                        if (results.Count > 0)
                        {
                            var addressComponents = results[0]["address_components"];
                            LocationInfo locationInfo = new LocationInfo();

                            foreach (var component in addressComponents)
                            {
                                var types = component["types"];
                                if (types.Any(t => t.ToString() == "country"))
                                {
                                    locationInfo.CountryName = component["long_name"].ToString();
                                }
                                else if (types.Any(t => t.ToString() == "administrative_area_level_1"))
                                {
                                    locationInfo.StateName = component["long_name"].ToString();
                                }
                                else if (types.Any(t => t.ToString() == "locality"))
                                {
                                    locationInfo.CityName = component["long_name"].ToString();
                                }
                            }

                            // Get timezone using another API or parse it from the JSON response
                            locationInfo.TimeZone = GetTimeZoneInfo(latitude, longitude);

                            return locationInfo;
                        }
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Error retrieving location data: " + ex.Message);
                }
            }
            return null;
        }
        public String GetTimeZoneInfo(double latitude, double longitude)
        {


            string apiUrl = $"{GoogleTimeZoneApiUrl}?location={latitude},{longitude}&timestamp={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&key={GoogleMapsApiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(apiUrl).Result; // Make a synchronous GET request
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                        string timeZoneId = data.timeZoneId;
                        string timeZoneName = data.timeZoneName;



                        return timeZoneId;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return null;
        }





        public class LocationInfo
        {
            public string CountryName { get; set; }
            public string CityName { get; set; }
            public string TimeZone { get; set; }
            public string StateName { get; set; }
        }

    }
}
