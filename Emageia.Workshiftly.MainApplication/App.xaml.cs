using Emageia.Workshiftly.Domain.Concrete;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Ninject;
using System;
using System.Threading.Tasks;
using System.Windows;
using Emageia.Workshiftly.Domain.Interface;
using System.Windows.Threading;
using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow;
using Emageia.Workshiftly.CoreFunction.IoC.Service.Http;
using System.Threading;
using Microsoft.Win32;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Reflection;
using Emageia.Workshiftly.Entity;
using Emageia.Workshiftly.CoreFunction.IoC.HalAccess;

using System.Net.Http;
using Emageia.Workshiftly.Entity.HttpClientModel;
using Newtonsoft.Json;
using Emageia.Workshiftly.Entity.HttpClientModel.DownloadFile;
using System.ServiceProcess;
using Emageia.Workshiftly.AutoUpdater.AutoUpdateExtensionService;
using Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using SocketIOClient;
using System.Collections.Generic;

namespace Emageia.Workshiftly.MainApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SharpUpdaterExtention updater;
        public SocketIO client;

        public static string tempFile;

       

        /// <summary>
        /// Custom startup so we load out IoC immediately before anything else
        /// </summary>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(OnPowerModeChanged);

            IKernel kernel = new StandardKernel();
            tempFile = Path.GetTempFileName();
            /// App is already running. Only one instance allowed to run
            /// 

            if (AnotherInstanceExists())
            {
                Process.GetCurrentProcess().Kill();
                return;
            }

            try
            {
                PathSet();
                //  environmentRead();
            }
            catch (Exception)
            {


            }


            initialApp();

            kernel.Bind<IClientDataStore>().To<ClientDataStore>();



            Thread InstallClient = new Thread(async () => await InstallJavaClient());
            InstallClient.Start();

            await ApplicationSetupAsync();

            // MyUninstallerOnBeforeUninstall();



            // Thread UnInstallClient = new Thread(async () => await MyUninstallerOnBeforeUninstall());
            // UnInstallClient.Start();

            //Thread UnInstallClient = new Thread(async () => await UnInstallJavaClient());
            //UnInstallClient.Start();



            try
            {

                DatabaseFacade facade = new DatabaseFacade(new ClientDataStoreDbContext());
                facade.EnsureCreated();


            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Success", "App.xaml OnStartup", ex.InnerException.Message.ToString(), ex.Message.ToString());


            }



            AutoRunWindowsStartup();

            //setup IOC Continer

            // Main Window
            Current.MainWindow = new MainWindow();
            //Current.MainWindow.Show();


        }

        private async Task SocketInitalition()
        {
            client = new SocketIO("ws://127.0.0.1:9001", new SocketIOOptions
            {
                Query = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYzMTAyYjI3LTVhYzctNGY5My04NzA2LTA0NGVmMDBhZWUxNyIsImVtYWlsIjoic2lyaXNlbmFAbWFpbGluYXRvci5jb20iLCJmaXJzdE5hbWUiOiJLdW1hcmEiLCJsYXN0TmFtZSI6IlNpcmlzZW5hIiwicm9sZSI6eyJpZCI6IjIxYmRmYTM1LTIyY2UtNDQyNi04NzMwLWZjNzRlZjhjZDY0OCIsIm5hbWUiOiJVU0VSIiwiZGVzY3JpcHRpb24iOiJ1c2VyIiwicGVybWlzc2lvbnMiOiJ7XCJrZXlcIjpcInVzZXJcIiwgXCJncmFudFR5cGVcIjogXCJhbGxcIiwgXCJkZXNjcmlwdGlvblwiOiBcIlwifSIsImlzQWN0aXZlIjoxLCJjcmVhdGVkQXQiOjE2MjA2NDMxNDQsInVwZGF0ZWRBdCI6MTYyMDY0MzE0NH0sImlzQWN0aXZlIjp0cnVlLCJpc0NsaWVudEFjdGl2ZSI6dHJ1ZSwiaXNBbGxvd09mZmxpbmVUYXNrIjp0cnVlLCJpYXQiOjE2Njk3ODIzNDMsImV4cCI6MTAwMDAxNjY5NzgyMzQzfQ.npziyzKueec96MdecmfmlMf19-T-bDFYXHEHrswZwsY"),
                    new KeyValuePair<string, string>("userId", "63102b27-5ac7-4f93-8706-044ef00aee17")
                }
            });

            client.OnConnected += async (sender, e) =>
            {


                // Emit a string
                await client.EmitAsync("activeWindowData:Post", "SocketInitalition");

                // Emit a string and an object
                //var dto = new TestDTO { Id = 123, Name = "bob" };
                //await client.EmitAsync("register", "source", dto);
            };
            await client.ConnectAsync();


        }
        public static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    CommonUtility.LogWriteLines("Success", "App.xaml OnPowerModeChanged", "PowerMode: OS is resuming from suspended state", "-----------");
                    MessageBox.Show("PowerMode: OS is resuming from suspended state");
                    break;

                case PowerModes.Suspend:
                    CommonUtility.LogWriteLines("Success", "App.xaml OnPowerModeChanged", "PowerMode: OS is about to be suspended", "-----------");
                    MessageBox.Show("PowerMode: OS is about to be suspended");
                    break;
            }
        }
        enum EnvironmentType { DEVELOPMENT, STAGING, PRODUCTION };
        private void environmentRead()
        {
            try
            {
                var Url = "";
                EnvironmentType environmentType;

                string BUID_ENVIRONMENT = ConfigurationSettings.AppSettings["BUID_ENVIRONMENT"];

                bool sucess = Enum.TryParse<EnvironmentType>(BUID_ENVIRONMENT, out environmentType);
                if (sucess)
                {
                    switch (environmentType)
                    {
                        case EnvironmentType.DEVELOPMENT:
                            CommonUtility.ServerPath = ConfigurationSettings.AppSettings["DEVELOPMENT_API"];
                            break;
                        case EnvironmentType.STAGING:
                            CommonUtility.ServerPath = ConfigurationSettings.AppSettings["STAGING_API"];
                            break;
                        case EnvironmentType.PRODUCTION:
                            CommonUtility.ServerPath = ConfigurationSettings.AppSettings["PRODUCTION_API"];
                            break;
                    }
                }
                else
                {
                  //  CommonUtility.ServerPath = "https://qa.workshiftly.com/api";
                  //  CommonUtility.ServerPath = "https://staging-api.workshiftly.com/api";
                    CommonUtility.ServerPath = "https://portal.workshiftly.com/api";
                    CommonUtility.LogWriteLines("Success", "App.xaml OnStartup", "", "f");

                }
            }
            catch (Exception ex)
            {
                //CommonUtility.LogWriteLine("////////////////////////*************   e.ReasonSessionEnding.ToString() ------->Application_SessionEnding workStatus = BREAK   " + e.ReasonSessionEnding.ToString() + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

                //CommonUtility.ServerPath = "https://qa.workshiftly.com/api";
               // CommonUtility.ServerPath = "https://staging-api.workshiftly.com/api";
                 CommonUtility.ServerPath = "https://portal.workshiftly.com/api";
                CommonUtility.LogWriteLines("Error", "App.xaml environmentRead", ex.Message.ToString(), "****");
            }




        }

        public static bool AnotherInstanceExists()
        {
            Process _currentRunningProcess = Process.GetCurrentProcess();
            Process[] _listOfProcs = Process.GetProcessesByName(_currentRunningProcess.ProcessName);

            foreach (Process _proc in _listOfProcs)
            {
                if ((_proc.MainModule.FileName == _currentRunningProcess.MainModule.FileName) && (_proc.Id != _currentRunningProcess.Id))
                {
                    return true;
                }
            }
            return false;
        }
        private async void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
           /// base.OnSessionEnding(e);
            CommonUtility.LogWriteLine("////////////////////////*************   e.ReasonSessionEnding.ToString() ------->Application_SessionEnding workStatus = BREAK   " + e.ReasonSessionEnding.ToString() + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

            string msg = string.Format("{0}. End session?", e.ReasonSessionEnding);
           // MessageBoxResult result = MessageBox.Show(msg, "Session Ending", MessageBoxButton.YesNo);

            
            if (IsLogeActiveUser())
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {
                    WorkStatusLog workStatusLog = new WorkStatusLog();

                    workStatusLog.userId = CommonUtility.UserSessions.id;
                    workStatusLog.deviceId = CommonUtility.DeviceSettings.id;
                    workStatusLog.userDate = DateTime.Now.ToString("yyy-MM-dd");
                    workStatusLog.date = CommonUtility.StartOfDay();
                    workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                    workStatusLog.workStatus = "BREAK";
                    workStatusLog.isBreakAutomaticallyStart = false;
                    workStatusLog.breakAutomaticallyStartDuration = 0;
                    workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                    workStatusLog.isSynced = false;


                    _ = await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                    await _clientDbContext.SaveChangesAsync();
                    CommonUtility.Status = "BREAK";
                    CommonUtility.LogWriteLine("////////////////////////*************   BREAK ------->Application_SessionEnding workStatus = BREAK   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

                }
            }
            else
            {
                // Cancel application shutdown to prevent Windows 7 killing
                // the process
                e.Cancel = true;

                // Terminate myself
                Shutdown();
            }

           
            
        }

        private bool IsLogeActiveUser()
        {
            return true;
        }

        private void PathSet()
        {
            try
            {
                var paths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var subFolderPath = System.IO.Path.Combine(paths, "Workshiftly Client");
                var subFolder = System.IO.Path.Combine(subFolderPath, "WorkshiftlyLog.txt");
                CommonUtility.LogPath = subFolder;
                CommonUtility.SubFolderPath = subFolderPath;
                var subFolderPathImage = System.IO.Path.Combine(subFolderPath, "Image");
                CommonUtility.ImagePath = subFolderPathImage;
                if (!Directory.Exists(subFolderPath))
                {
                    Directory.CreateDirectory(subFolderPath);
                }
                if (!Directory.Exists(subFolderPathImage))
                {
                    Directory.CreateDirectory(subFolderPathImage);
                }

                var os = Environment.OSVersion;
                CommonUtility.LogWriteLine("\n \n ************************* Starting Now Application" + DateTime.Now + "*********************** \n \n");
                //var nid = APIFuncs.GetMacAddress().ToString();
                CommonUtility.LogWriteLine("\n \n ********** IP and Mac " + DateTime.Now
                    + "\n \n ************************* Mac Address = " + APIFuncs.GetMacAddress()
                    + "\n ************************* Mac Address Old = " + APIFuncs.GetMacAddressOld()
                    + "\n ************************* System Mac ID = " + APIFuncs.GetSystemMACID()
                    + "\n ************************* Ip Address = " + APIFuncs.GetIpAddress()
                    + "\n ************************* Environment.MachineName = " + Environment.MachineName
                    + "\n ************************* Environment.Environment.UserName = " + Environment.UserName + "\n \n"

                    + "\n \n ************************* Current OS Information:\n"
                    + "\n ************************* Platform:  " + os.Platform
                    + "\n ************************* Os Name:  " + APIFuncs.GetOSFriendlyName()
                    + "\n ************************* Os Name 2:  " + APIFuncs.FriendlyName()
                    + "\n ************************* Version String: = " + os.VersionString + "\n"

                    + "\n ************************* Ip Address = "
                    + "\n *************************      Major = " + os.Version.Major
                    + "\n *************************      Minor = " + os.Version.Minor
                    + "\n ************************* Service Pack: = " + os.ServicePack + "\n \n"

                    );

                string name = string.Format("{0,20}{1,20}{2,29}{3,70}{4,26}",
                        "Date", "Type", "Fuction", "Result", "Error Description");
                CommonUtility.LogWriteLine(name, "");

                // CommonUtility.LogWriteLine("Path Set");
            }
            catch (Exception ex)
            {

            }

        }
        private static void JavaClientApplicationKill()
        {
            //  Process[] workers = Process.GetProcessesByName("workshiftly-desktop-client");
            Process[] workers = Process.GetProcessesByName("WorkShiftly");
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }
        }


        #region JavaClient install

        private static async Task InstallJavaClient()
        {
            try
            {
                CommonUtility.LogWriteLines("Success", "App.xaml InstallJavaClient", " Java Client Exit Checking Start", "*********************");

                //query to get all installed products
                //var ProgramName = "workshiftly-desktop-client";
                var ProgramName = "WorkShiftly";
                //load the query string
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                //get the specified proram(s)
                ManagementObject managementObject = null;
                var exit = false;
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        
                        //make sure that we are uninstalling the correct application
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            managementObject = mo;
                           // mo.InvokeMethod("Uninstall", null);
                            CommonUtility.LogWriteLines("Success", "App.xaml InstallJavaClient", " allready Java Client Exit --> " + mo["Version"].ToString(), "*********************");
                            exit = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        CommonUtility.LogWriteLines("Error", "App.xaml InstallJavaClient", " Java Client Exit Checking Error", ex.ToString());

                    }
                }

                if (!exit)
                {
                    //await Task.Run(() => DeployJavaClientApplications());

                    await Task.Run(() => RunInstallMSI());


                }
                else
                {
                    //Thread UnInstallClient = new Thread(async () => await UnInstallJavaClient(managementObject));
                    //UnInstallClient.Start();

                    (bool success, string newMSIPath, string sMSIPath) = await UnInstallJavaClient(managementObject);
                    if (success == true && newMSIPath != null)
                    {

                        if (File.Exists(newMSIPath))
                        {
                            // CommonUtility.LogWriteLine("New Downolad File Exite");
                            RunDownloadInstallMSIRun(newMSIPath);

                        }
                        else if (File.Exists(sMSIPath) && !File.Exists(newMSIPath))
                        {
                            //  CommonUtility.LogWriteLine("before File.Exists(sMSIPath)");
                            RunDownloadInstallMSIRun(sMSIPath);

                        }
                        else
                        {

                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }


        public static void RunInstallMSI()
        {
            try
            {
                CommonUtility.LogWriteLines("Success", "App.xaml RunInstallMSI", " Java Client Start installing", "****");

              //  var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\workshiftly-desktop-client-1.4.6.msi";
                var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\WorkShiftly-2.6.0.msi";
                Process process = new Process();
                process.StartInfo.FileName = "msiexec.exe";
                //process.StartInfo.Arguments = string.Format(" /qb /i \"{0}\" ALLUSERS=1", sMSIPath);
                process.StartInfo.Arguments = string.Format(" /qb /i \"{0}\" /quiet", sMSIPath);
                process.Start();
                process.WaitForExit();
                CommonUtility.LogWriteLines("Success", "App.xaml RunInstallMSI", " Java clientApplication installed successfully!", "****");
                javaClientAutoStart();
                // return true; //Return True if process ended successfully
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "App.xaml RunInstallMSI", " There was a problem installing the application!", ex.ToString());

                //Return False if process ended unsuccessfully
            }
        }

        public static void RunDownloadInstallMSIRun(string path)
        {
            try
            {
                CommonUtility.LogWriteLines("Success", "App.xaml RunDownloadInstallMSIRun", " Java Client Start installing", "****");
                //var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\workshiftly-desktop-client-1.4.6.msi";

                var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\WorkShiftly-2.6.0.msi";
                sMSIPath = path;
                Process process = new Process();
                process.StartInfo.FileName = "msiexec.exe";
                //process.StartInfo.Arguments = string.Format(" /qb /i \"{0}\" ALLUSERS=1", sMSIPath);
                process.StartInfo.Arguments = string.Format(" /qb /i \"{0}\" /quiet", sMSIPath);
                process.Start();
                process.WaitForExit();
                CommonUtility.LogWriteLines("Success", "App.xaml RunDownloadInstallMSIRun", " Java clientApplication installed successfull", "****");

                // return true; //Return True if process ended successfully
            }
            catch
            {
                CommonUtility.LogWriteLines("Error", "App.xaml RunDownloadInstallMSIRun", " Java clientApplication installed successfully!", "****");
                // return false; //if process ended unsuccessfully
            }
        }


        private static void javaClientAutoStart()
        {
            // Get the object on which the method will be invoked
            ManagementClass processClass =
                new ManagementClass("Win32_Process");

            // Get an input parameters object for this method
            ManagementBaseObject inParams =
                processClass.GetMethodParameters("Create");

            var no = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //var subFolderPatth = System.IO.Path.Combine(no, @"workshiftly-desktop-client\workshiftly-desktop-client.exe");
            var subFolderPatth = System.IO.Path.Combine(no, @"WorkShiftly\WorkShiftly.exe");

            // Fill in input parameter values
            // inParams["CommandLine"] = @"C:\Users\Niroshan\AppData\Local\workshiftly-desktop-client\workshiftly-desktop-client.exe";
            inParams["CommandLine"] = subFolderPatth;

            // Method Options
            InvokeMethodOptions methodOptions = new
                InvokeMethodOptions(null,
                System.TimeSpan.MaxValue);

            // Execute the method
            ManagementBaseObject outParams =
                processClass.InvokeMethod("Create",
                inParams, methodOptions);

            // Display results
            // Note: The return code of the method is
            // provided in the "returnValue" property
            // of the outParams object
            //Console.WriteLine(
            //    "Creation of calculator process returned: "
            //    + outParams["returnValue"]);
            //Console.WriteLine("Process ID: "
            //    + outParams["processId"]);

            //Console.Read();
        }
        private void AutoRunWindowsStartup()
        {
            try
            {
                var currentPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                var DbPath = System.IO.Path.Combine(currentPath, "Emageia.Workshiftly.MainApplication.exe");


                using (RegistryKey Key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\"))
                    if (Key != null)
                    {
                        string val = (string)Key.GetValue("Workshiftly", DbPath);
                        if (val != null)
                        {
                            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                            reg.SetValue("Workshiftly", DbPath);
                        }

                    }
                    else
                    {
                        CommonUtility.LogWriteLines("error else", "App.xaml AutoRunWindowsStartup", " key not found", "****");

                        // MessageBox.Show("key not found");
                    }


            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "App.xaml RunDownloadInstallMSIRun", " key not found", ex.Message.ToString());

                  string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "AutoRunWindowsStartup" + ex.Message.ToString() };
                  File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
            }


        }
        #endregion



        #region helpers



        public void initialApp()
        {

            ISharpUpdatable applicationInfos = new ISharpUpdatable();
            applicationInfos.ApplicationName = "Emageia.Workshiftly.MainApplication";
            applicationInfos.ApplicationVersion = "Emageia.Workshiftly.MainApplication";
            applicationInfos.ApplicationId = "Emageia.Workshiftly.MainApplication";
            applicationInfos.ApplicationAssembly = Assembly.GetExecutingAssembly();
            applicationInfos.UpdateXmlLocation = new Uri("http://localhost:8000/Api/TemplateDocument/DownlodDoc?fileName=AutoUpdate.xml");
            updater = new SharpUpdaterExtention(applicationInfos);


            // updater.DoUpdate();

        }

        private async Task ApplicationSetupAsync()
        {
            /// SetEnvironmentVariableForUpdateWpf(true, true);
            //  updater.DoUpdate();


        }



        public static void DeployJavaClientApplications()
        {

            try
            {
                CommonUtility.LogWriteLine("Deploy Java Client Applications");
               // var resourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\workshiftly-desktop-client-1.4.6.msi";

                var resourcePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\WorkShiftly-2.6.0.msi";
                string UninstallCommandString = "/I  {0} /quiet";
                // string strPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory)+"\\workshiftly-desktop-client-1.4.6.msi";
                // Directory.SetCurrentDirectory("C:\\Program Files (x86)\\Default Company Name\\Resource\\EmageaSetup\\workshiftly-desktop-client-1.4.6.msi");
               // var path = "C:\\Program Files (x86)\\Default Company Name\\MyNew Folder\\Resource\\workshiftly-desktop-client-1.4.6.msi";
                //var path = "C:\\Program Files (x86)\\Default Company Name\\MyNew Folder\\Resource\\WorkShiftly-2.6.0.msi";

                var path = "C:\\Program Files (x86)\\WorkShiftlyClient";

                if(Directory.Exists(path))
                {
                    path= "C:\\Program Files (x86)\\WorkShiftlyClient\\Resource";
                    if (Directory.Exists(path))
                    {
                        path = "C:\\Program Files (x86)\\WorkShiftlyClient\\Resource\\WorkShiftly-2.6.0.msi";
                    }
                }

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                process.StartInfo = startInfo;

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardError = true;

                startInfo.FileName = "msiexec.exe";
                startInfo.Arguments = string.Format(UninstallCommandString, path);
                CommonUtility.LogWriteLine(path);
                CommonUtility.LogWriteLine(UninstallCommandString);
                process.Start();

            }
            catch (Exception ex)
            {
                var message = string.Format("Deploy Java Client Applications Error occured: {0}", ex.InnerException);
                CommonUtility.LogWriteLine(message);
                // CommonUtility.LogWriteLine("Error occured: {0}", ex.InnerException);

            }


        }




        private static async Task<(bool, string, string)> UnInstallJavaClient(ManagementObject managementObject)
        {
            try
            {
                // CommonUtility.LogWriteLines("Success", "App.xaml UnInstallJavaClient", " allready Java Client Exit" + "Update function fire", "****");
                var update = false;
                //var version = (object)null;
                var version = "";
                //query to get all installed products
               // var ProgramName = "workshiftly-desktop-client";
                var ProgramName = "WorkShiftly";
                // var sMSIPath = "";
                ManagementObject unInstall = new ManagementObject();
                //load the query string

                //make sure that we are uninstalling the correct application
                if (managementObject["Name"].ToString() == ProgramName)
                {
                    version = managementObject["Version"].ToString();
                    unInstall = managementObject;
                    update = true;
                    CommonUtility.LogWriteLines("Success", "App.xaml UnInstallJavaClient", " Java Client uninstall", "****");
                    // managementObject.InvokeMethod()
                }


                if (update && SharpUpdateXml.ExistsOnServer(new Uri("http://localhost:8000/Api/updates/update-available")))
                {
                    JavaClientApplicationKill();

                    //if (SharpUpdateXml.ExistsOnServer(new Uri("https://localhost:44300/Api//updates/update-available")))

                    // CommonUtility.LogWriteLine("222222222 Java Client uninstall -------------------->");
                    try
                    {
                        // if (unInstall != null) unInstall.InvokeMethod("Uninstall", null);
                        //  else return;


                        var returnData = new ServerReturnUrlObject();
                        using (HttpClient client = new HttpClient())
                        {

                            HttpResponseMessage response = client.GetAsync(new Uri("http://localhost:8000/Api/updates/update-available")).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var returnUrls = await response.Content.ReadAsStringAsync();
                                returnData = JsonConvert.DeserializeObject<ServerReturnUrlObject>(returnUrls);

                                try
                                {
                                    if (!returnData.error && (Version.Parse(version.ToString()) < Version.Parse(returnData.version)))
                                    {
                                       // var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\workshiftly-desktop-client-{version}.msi";
                                      //  var newMSIPath = Path.Combine(CommonUtility.SubFolderPath + $"\\workshiftly-desktop-client-{returnData.version}.msi");

                                        var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\WorkShiftly-{version}.msi";
                                        var newMSIPath = Path.Combine(CommonUtility.SubFolderPath + $"\\WorkShiftly-{returnData.version}.msi");
                                        // await Task.Run(() => RunDownloadInstallMSI(new Uri(returnData.url), returnData.version, managementObject));
                                        // byte[] returnFileByteArray = await RunDownloadInstallMSI(new Uri(returnData.url), returnData.version);
                                        try
                                        {
                                            await RunDownloadInstallMSI2(new Uri(returnData.url), returnData.version, GetTempFile());
                                            return (true, newMSIPath, sMSIPath);
                                        }
                                        catch (Exception ex)
                                        {

                                            CommonUtility.LogWriteLines("Error", "App.xaml UnInstallJavaClient", ex.Message.ToString(), "****");

                                        }

                                    }

                                    return (false, null, null);
                                }
                                catch (Exception ex)
                                {
                                    CommonUtility.LogWriteLines("Error", "App.xaml UnInstallJavaClient", ex.Message.ToString(), "****");
                                    return (false, null, null);
                                }
                            }
                            else
                            {
                                CommonUtility.LogWriteLines("Error", "App.xaml UnInstallJavaClient", " Java client Application download Error", "****");
                                return (false, null, null);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        CommonUtility.LogWriteLines("Success", "App.xaml UnInstallJavaClient", ex.ToString(), ex.Message.ToString());
                        return (false, null, null);
                    }

                }
                CommonUtility.LogWriteLines("Success", "App.xaml UnInstallJavaClient", "Up to date Application or Server not exist", "****");
                return (false, null, null);

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("error", "App.xaml UnInstallJavaClient", " Java clientApplication installed successfull", "****");

                return (false, null, null);
            }
        }

        public static string GetTempFile()
        {
            return tempFile;
        }

        public async static Task RunDownloadInstallMSI2(Uri url, string version, string tempFile)
        {

            try
            {
                var _httpClient = new HttpClient();
               // var sMSIPath = Path.Combine(CommonUtility.SubFolderPath + $"\\workshiftly-desktop-client-{version}.msi");
                var sMSIPath = Path.Combine(CommonUtility.SubFolderPath + $"\\WorkShiftly-{version}.msi");
                byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);
                File.WriteAllBytes(sMSIPath, fileBytes);
                CommonUtility.LogWriteLines("Success", "App.xaml RunDownloadInstallMSI2", "Write File", "****");

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("error", "App.xaml RunDownloadInstallMSI2", " Java clientApplication installed successfull", "****");
            }
        }
        public async static Task<byte[]> RunDownloadInstallMSI(Uri url, string version)
        {

            try
            {
                var _httpClient = new HttpClient();
               // var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\workshiftly-desktop-client-{version}.msi";
                var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\WorkShiftly-{version}.msi";

                byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);
                if (File.Exists(sMSIPath))
                {
                    File.Delete(sMSIPath);

                }
                else if (fileBytes != null)
                {
                    File.WriteAllBytes(sMSIPath, fileBytes);
                    CommonUtility.LogWriteLine("RunDownloadInstallMSI!" + sMSIPath);

                }
                else
                {

                }
                return fileBytes;
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("error", "App.xaml RunDownloadInstallMSI", ex.InnerException.ToString(), ex.InnerException.ToString());
                return null;
            }
        }



        #endregion

        #region Encryption and Decryption
        public string Encrypt(string text)
        {
            try
            {
                //string textToEncrypt = "Water";
                string textToEncrypt = text;
                string ToReturn = "";
                string publickey = "Niroshan";
                string secretkey = "Sameera Emageia Workshiftly";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("error", "App.xaml UnInstallJavaClient", " Java clientApplication installed successfull", "****");

                throw new Exception(ex.Message, ex.InnerException);
            }
        }



        public string Decrypt(string text)
        {
            try
            {
                // string textToDecrypt = "VtbM/yjSA2Q=";
                string textToDecrypt = text;
                string ToReturn = "";
                string publickey = "Niroshan";
                string privatekey = "Sameera Emageia Workshiftly";
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(privatekey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }


        #endregion



        #region Never user code 
        //var Name = mo["Name"].ToString();
        //var AssignmentType = mo["AssignmentType"].ToString();
        //var Caption = mo["Caption"];
        //var Description = mo["Description"];
        //var IdentifyingNumber = mo["IdentifyingNumber"];
        //var InstallLocation = mo["InstallLocation"];

        //var InstallState = mo["InstallState"];
        //var HelpLink = mo["HelpLink"];
        //var HelpTelephone = mo["HelpTelephone"];
        //var InstallSource = mo["InstallSource"];
        //var Language = mo["Language"];
        //var LocalPackage = mo["LocalPackage"];
        //var PackageCache = mo["PackageCache"];
        //var PackageCode = mo["PackageCode"];
        //var PackageName = mo["PackageName"];

        //var ProductID = mo["ProductID"];
        //var RegOwner = mo["RegOwner"];
        //var RegCompany = mo["RegCompany"];
        //var SKUNumber = mo["SKUNumber"];
        //var Transforms = mo["Transforms"];
        //var URLInfoAbout = mo["URLInfoAbout"];
        //var URLUpdateInfo = mo["URLUpdateInfo"];
        //var Vendor = mo["Vendor"];
        //var WordCount = mo["WordCount"];
        //var Version = mo["Version"];
        private async Task MyUninstallerOnBeforeUninstall()
        {
            try
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "My Uninstaller On Before Uninstaller" };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);


                //var ProgramName = "workshiftly-desktop-client";
                var ProgramName = "Workshiftly";
                //load the query string
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                //get the specified proram(s)
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        //make sure that we are uninstalling the correct application
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            var nadw = mo["IdentifyingNumber"].ToString();
                            UnInstallDeployApplications(mo["IdentifyingNumber"].ToString());
                            //call to Win32_Product Uninstall method, no parameters needed
                            ///  object hr = mo.InvokeMethod("Uninstall", null);
                            // return (bool)hr;
                        }
                    }
                    catch (Exception ex)
                    {
                        //this program may not have a name property, so an exception will be thrown
                    }
                }
                //was not found...
                // return false;
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("error", "App.xaml UnInstallJavaClient", " Java clientApplication installed successfull", "****");

                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "My Uninstaller On Before Uninstaller eeeeeeeeeeeeeeee " + ex.Message.ToString() };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
            }
        }


        public static void UnInstallDeployApplications(string uuid)
        {

            try
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "UnInstallDeployApplications function 2222222222222222" + uuid };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
                //var ne = "workshiftly-desktop-client";
                var ne = "Workshiftly";
                string UninstallCommandString = "/x {0} /qn";
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                process.StartInfo = startInfo;

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardError = true;

                //startInfo.FileName = "msiexec.exe";
                //startInfo.Arguments = string.Format(UninstallCommandString, uuid);  
                //startInfo.FileName = "javaws";
                //startInfo.Arguments = "uninstall";

                process.Start();

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("error", "App.xaml UnInstallJavaClient", " Java clientApplication installed successfull", "****");

                Console.WriteLine("Error occured: {0}", ex.InnerException);
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + ex.InnerException + "\t" + "UnInstallDeployApplications eeeeeeeeeeeeeeerrrrrrrrrorrrrrrrrrr" };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);

            }
            finally
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "UnInstallDeployApplications function finally 3333333333333333333333333" };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
            }

        }



        private async Task ActiveWindowUtility()
        {

            var activeWindowUtitliy = new ActiveWindowUtility();

            CommonUtility.ActiveWindoDispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            CommonUtility.ActiveWindoDispatcherTimer.Tick += async delegate (object sender, EventArgs e)
            {
                //if (CommonUtility.HasLogging)
                //{
                activeWindowUtitliy.activeWindow();

                //}
                // HttpRequestCaller.syncActivityWindows();
            };

            CommonUtility.ActiveWindoDispatcherTimer.Start();


        }

        private async Task SyncAtivityWindows()
        {
            CommonUtility.syncActiveWindow.Start();
            var _activeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5)
            };
            _activeTimer.Tick += async delegate (object sender, EventArgs e)
            {

                // CommonUtility.syncActiveWindow = new Thread(() => HttpRequestCaller.syncActivityWindows());
                // CommonUtility.syncActiveWindow

                //if (!CommonUtility.syncActiveWindow.IsAlive && CommonUtility.HasLogging)
                //{
                //    CommonUtility.syncActiveWindow.Start();
                //}
                //CommonUtility.syncActiveWindow.Start();
                Thread syncActiveWindow = new Thread(() => HttpRequestCaller.syncActivityWindows());
                syncActiveWindow.Start();

                Thread syncScreenShot = new Thread(() => HttpRequestCaller.syncScreenshots());
                syncScreenShot.Start();

                //  HttpRequestCaller.syncActivityWindows();
            };
            _activeTimer.Start();

        }


        private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
        {
            string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = String.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }

        private static void Deploye()
        {
            //var path = "C:\\Program Files (x86)\\Default Company Name\\MyNew Folder\\Resource\\workshiftly-desktop-client-1.4.6.msi";
            //var path = "C:\\Program Files (x86)\\Default Company Name\\MyNew Folder\\Resource\\WorkShiftly-2.6.0.msi";
            var path = "C:\\Program Files (x86)\\WorkShiftlyClient\\Resource\\WorkShiftly-2.6.0.msi";
            string argument = "/C Choice /C Y /N /D Y /T 4 & Install /S /F  \"{0}\" /Q ";

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = String.Format(argument, path);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }

        //private async Task ApplicationSetupAsync()
        //{


        //    new DefaultFrameworkConstruction()
        //        .UseClientDataStore()
        //        .Build();

        //    IoC.Setup();

        //    (IClientDataStore)Framework.Provider.GetService(typeof(IClientDataStore));
        //    //var client = Framework.Service<IClientDataStore>();
        //    //await IoC.IClientDataStore.En

        //}
        #endregion

    }
}
