using Emageia.Workshiftly.ProcessExtensionsService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Timers = System.Timers;

namespace Emageia.Workshiftly.RunnableService
{
    partial class RunnableService : ServiceBase
    {

        Timers.Timer timer1 = new Timers.Timer();
        //int SheduleTime = Convert.ToInt32(ConfigurationSettings.AppSettings["ThreadTime"]);
        private Thread Worker = null;
        private static string SystemBinUrl = AppDomain.CurrentDomain.BaseDirectory;
        public RunnableService()
        {
            InitializeComponent();

            //  SetEnvironmentVariableForUpdateWpf(false,false);
        }

        #region Encryption and Decryption

        public const string RESOURCEFOLDERNAME = @"Resource\CSharpClient";
        public const string CLIENTFOLDERNAME = @"Appliction\Client";
        public const string RESOURCECLIENTFOLDERNAME = @"Appliction\Client\Release";
        public const string CLIENTFILEINSTALLERNAME = @"Appliction\Client\Release\Emageia.Workshiftly.MainApplication.exe";


        public static void SetEnvironmentVariableForUpdateWpf(bool updatingServer, bool extract)
        {

            var getDetails = GetEnvironmentVariableForUpdateWpf();
            string user = nameof(user);


            if (getDetails != null)
            {
                jsonEncrypt(false, false, false, "", SystemBinUrl + RESOURCEFOLDERNAME + @"\Release.zip", false, user);
                //return;

            }
            else if (!getDetails.UpdatingWpf && !getDetails.UpdatingServer && getDetails.SuccessfulyDownload && updatingServer)
            {
                jsonEncrypt(false, updatingServer, false, getDetails.downloadpath, SystemBinUrl + RESOURCEFOLDERNAME + @"\Release.zip", true, user);
                //return ;
            }
            else if (getDetails.UpdatingServer && !updatingServer && getDetails.SuccessfulyDownload && !extract)
            {
                jsonEncrypt(false, updatingServer, false, getDetails.downloadpath, SystemBinUrl + RESOURCEFOLDERNAME + @"\Release.zip", extract, user);
                // return;
            }

            //return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updatingWpf"></param>
        /// <param name="updatingServer"></param>
        /// <param name="SuccessfulyDownload"></param>
        /// <param name="downloadpath"></param>
        /// <param name="storedPath"></param>
        /// <param name="extract"></param>
        /// <param name="user"></param>
        private static void jsonEncrypt(bool updatingWpf, bool updatingServer, bool SuccessfulyDownload, string downloadpath, string storedPath, bool extract, string user)
        {
            try
            {
                SyncUpdateStatus updateStatus = new SyncUpdateStatus()
                {
                    Id = 1,
                    UpdatingWpf = updatingWpf,
                    UpdatingServer = updatingServer,
                    SuccessfulyDownload = SuccessfulyDownload,
                    Extract = extract,
                    downloadpath = downloadpath,
                    storedPath = storedPath
                };
                var json = JsonConvert.SerializeObject(updateStatus, Formatting.None);
                string userEnvVars = Encrypt(json);
                Environment.SetEnvironmentVariable(user, userEnvVars,
                                          EnvironmentVariableTarget.User);
            }
            catch (Exception)
            {

                throw;
            }


        }
        public static SyncUpdateStatus GetEnvironmentVariableForUpdateWpf()
        {

            string user = nameof(user);
            var value = Environment.GetEnvironmentVariable(user, EnvironmentVariableTarget.User);
            var json = Decrypt(value);

            return JsonConvert.DeserializeObject<SyncUpdateStatus>(json);
        }



        public static string Encrypt(string text)
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
                throw new Exception(ex.Message, ex.InnerException);
            }
        }



        public static string Decrypt(string text)
        {
            try
            {
                // string textToDecrypt = "VtbM/yjSA2Q=";
                string textToDecrypt = text;
                string ToReturn = "";
                string publickey = "niroshan";
                string privatekey = "sameera emageia workshiftly";
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
        protected override void OnStart(string[] args)
        {
            try
            {
                //timer1.Enabled = true;
                //timer1.Elapsed += new Timers.ElapsedEventHandler(timer1_Elapsed);
                //timer1.Interval = 3000;
                //timer1.Start();

                ThreadStart start = new ThreadStart(Working);
                Worker = new Thread(start);
                Worker.Start();
            }
            catch (Exception exp)
            {

            }
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStop()
        {
            try
            {

                if ((Worker != null) & Worker.IsAlive)
                {
                    Worker.Abort();
                }


                timer1.Stop();
            }
            catch
            {
                throw;
            }

        }


        private void timer1_Elapsed(object sender, EventArgs e)
        {
            try
            {

                if (!AnotherInstanceExists())
                {

                    string str = @"C:\Users\Niroshan\Desktop\My\wpf\WpfBasics\WpfBasics\bin\Debug\WpfBasics.exe";
                    string sts = @"C:\Users\Niroshan\Desktop\emageia\WorkshiftlyClientC#\workshiftly-silent-tracking\Emageia.Workshiftly.MainApplication\bin\Release\Emageia.Workshiftly.MainApplication.exe";

                    //  ProcessExtensions.StartProcessAsCurrentUser(sts);
                   
                }

            }
            catch (Exception)
            {


            }
        }


        private void Working()
        {
            try
            {
                while (true)
                {
                    // var getDetails = GetEnvironmentVariableForUpdateWpf();
                    try
                    {
                        //if (getDetails.SuccessfulyDownload && File.Exists(getDetails.downloadpath))
                        //{
                        //    SetEnvironmentVariableForUpdateWpf(true, true); 

                        //}
                        //else
                        //{
                        extartctFolder();
                        // }


                    }
                    catch (Exception es)
                    {

                        //  throw;
                    }
                    //if (AnotherInstanceExists())
                    //{
                    //   var IOSIO=  GetProcessPath("Emageia.Workshiftly.MainApplication");
                    //}
                    // if (!AnotherInstanceExists() && File.Exists(SystemBinUrl + CLIENTFILEINSTALLERNAME) && !getDetails.UpdatingServer)
                    if (!AnotherInstanceExists() && File.Exists(SystemBinUrl + CLIENTFILEINSTALLERNAME))
                    {

                        string str = @"C:\Users\Niroshan\Desktop\My\wpf\WpfBasics\WpfBasics\bin\Debug\WpfBasics.exe";
                        string sts = @"C:\Users\Niroshan\Desktop\emageia\WorkshiftlyClientC#\workshiftly-silent-tracking\Emageia.Workshiftly.MainApplication\bin\Release\Emageia.Workshiftly.MainApplication.exe";
                        var niou = SystemBinUrl + CLIENTFILEINSTALLERNAME;
                        bool appStartOrNot = ProcessExtensions.StartProcessAsCurrentUser(SystemBinUrl + CLIENTFILEINSTALLERNAME);
                        //ActivityLogWriter.LogWrite("Forex App is already running.Only one instance allowed to run", true, "User", Globals.kioskID, Globals.PassportNo);
                        // MessageBox.Show("Forex App is already running.", "Only one instance allowed to run", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }



                    Thread.Sleep(5000);
                }

            }
            catch (Exception ex)
            {

            }
        }



        public string GetProcessPath(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            try
            {
                Process element = (from p in Process.GetProcesses()
                                   where p.ProcessName == name
                                   select p).FirstOrDefault();
                var ii = GetProcessPath(element.Id);
                if (processes.Length > 0)
                {
                    var OIJ = processes[0].MainModule.FileName;
                    return processes[0].MainModule.FileName;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception es)
            {

                throw;
            }

        }

        public static string GetProcessPath(int processId)
        {
            string MethodResult = "";
            try
            {
                string Query = "SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;

                using (ManagementObjectSearcher mos = new ManagementObjectSearcher(Query))
                {
                    using (ManagementObjectCollection moc = mos.Get())
                    {
                        string ExecutablePath = (from mo in moc.Cast<ManagementObject>() select mo["ExecutablePath"]).First().ToString();

                        MethodResult = ExecutablePath;

                    }

                }

            }
            catch //(Exception ex)
            {
                //ex.HandleException();
            }
            return MethodResult;
        }

        private static void extartctFolder()
        {
            try
            {
                if (!Directory.Exists(SystemBinUrl + CLIENTFOLDERNAME))
                {
                    Directory.CreateDirectory(SystemBinUrl + CLIENTFOLDERNAME);
                }
                if (File.Exists(SystemBinUrl + RESOURCEFOLDERNAME + @"\Release.zip"))
                {

                    if (!Directory.Exists(SystemBinUrl + RESOURCECLIENTFOLDERNAME))
                    {
                        UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                    }
                    else if (getDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME) == 0)
                    {
                        UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                    }
                    else if (getDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME) < 84)
                    {
                        deleteDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME);
                        UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                    }

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

        }

        private static void extartctFolder1()
        {
            try
            {
                var getDetails = GetEnvironmentVariableForUpdateWpf();

                if (!Directory.Exists(SystemBinUrl + CLIENTFOLDERNAME))
                {
                    Directory.CreateDirectory(SystemBinUrl + CLIENTFOLDERNAME);
                }
                if (File.Exists(SystemBinUrl + RESOURCEFOLDERNAME + @"\Release.zip"))
                {
                    // SetEnvironmentVariableForUpdateWpf(true, true);
                    //if (getDetails.SuccessfulyDownload)
                    //{
                    //JavaClientApplicationKill();
                    //SetEnvironmentVariableForUpdateWpf(true, true);
                    //deleteDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME);
                    //UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                    //SetEnvironmentVariableForUpdateWpf(false, false);
                    //}
                    //else
                    //{
                    if (!Directory.Exists(SystemBinUrl + RESOURCECLIENTFOLDERNAME))
                    {
                        //  SetEnvironmentVariableForUpdateWpf(true, true);
                        UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                        //   SetEnvironmentVariableForUpdateWpf(false, false);
                    }
                    else if (getDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME) == 0)
                    {
                        //  SetEnvironmentVariableForUpdateWpf(true, true);
                        UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                        //  SetEnvironmentVariableForUpdateWpf(false, false);
                    }
                    else if (getDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME) < 84)
                    {
                        // SetEnvironmentVariableForUpdateWpf(true, true);
                        deleteDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME);
                        UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                        // SetEnvironmentVariableForUpdateWpf(false, false);
                    }

                    //}

                }
                else
                {
                    //if (getDetails.SuccessfulyDownload)
                    //{
                    //    SetEnvironmentVariableForUpdateWpf(true, true);
                    //    JavaClientApplicationKill();
                    //    deleteDirectoryCount(SystemBinUrl + RESOURCECLIENTFOLDERNAME);
                    //    UnZipFile(SystemBinUrl + RESOURCEFOLDERNAME, SystemBinUrl + CLIENTFOLDERNAME);
                    //    SetEnvironmentVariableForUpdateWpf(false, false);
                    //}

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

        }

        private static int getDirectoryCount(string sourcePath)
        {
            var countDirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories).Count();
            var countFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Count();
            return countDirectories + countFiles;
        }

        private static void deleteDirectoryCount(string sourcePath)
        {
            DirectoryInfo di = new DirectoryInfo(sourcePath);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo subDirectory in di.EnumerateDirectories())
            {
                subDirectory.Delete(true);
            }
            foreach (DirectoryInfo subDirectory in di.EnumerateDirectories())
            {
                subDirectory.Delete(true);
            }


        }
        public static bool AnotherInstanceExists()
        {
            Process _currentRunningProcess = Process.GetCurrentProcess();
            Process[] _listOfProcs = Process.GetProcessesByName("Emageia.Workshiftly.MainApplication");
            //  Process[] _listOfProcs = Process.GetProcessesByName("WpfBasics");
            if (_listOfProcs.Length > 0)
            {
                return true;
            }

            return false;
        }


        private static void JavaClientApplicationKill()
        {
            //Process[] workers = Process.GetProcessesByName("workshiftly-desktop-client");
            Process[] workers = Process.GetProcessesByName("WorkShiftly");
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }
        }


        private static void CreateZipFile(string ZipedFileFolder, string SourceFolder)
        {
            // Create and open a new ZIP file
            string zipFileName = string.Format("zipfile-{0:yyyy-MM-dd_hh-mm-ss-tt}.zip", DateTime.Now);
            string zipFilepath = System.IO.Path.Combine(ZipedFileFolder, zipFileName);
            var zip = ZipFile.Open(zipFilepath, ZipArchiveMode.Create);
            string[] filesToZip = Directory.GetFiles(SourceFolder, "*.txt", SearchOption.AllDirectories);
            foreach (var file in filesToZip)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, System.IO.Path.GetFileName(file), CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }

        /// <summary>
        /// Extract File
        /// </summary>
        /// <param name="ZipedFileFolder"> Ziped file Path</param>
        /// <param name="extractPath"> Extract File Path</param>
        private static void UnZipFile(string ZipedFileFolder, string extractPath)
        {
            //how to unzip a existing zip file to a folder
            string zipPath = Directory.GetFiles(ZipedFileFolder, "*.zip", SearchOption.AllDirectories).FirstOrDefault();
            //string extractPath = @"E:\Extract";

            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }
    }



    public class SyncUpdateStatus
    {
        public int Id { get; set; }
        public bool UpdatingWpf { get; set; }
        public bool UpdatingServer { get; set; }
        public bool SuccessfulyDownload { get; set; }
        public bool Extract { get; set; }
        public string downloadpath { get; set; }
        public string storedPath { get; set; }
    }

}
