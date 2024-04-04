//using Emageia.Workshiftly.CoreFunction.IoC.Utility;
//using Emageia.Workshiftly.Entity.HttpClientModel.DownloadFile;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Management;
//using System.Net;
//using System.Net.Http;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Emageia.Workshiftly.CoreFunction.IoC.Update
//{
//    public class Updater
//    {
        
//        private BackgroundWorker bgWorker;
//        List<DownloadFileInfo> downloadFileListTemp = null;
//        private bool bNeedRestart = false;
//        private bool bDownload = false;
//        public SharpUpdater(ISharpUpdatable applicationInfo)
//        {
//            this.applicationInfo = applicationInfo;
//            this.bgWorker = new BackgroundWorker();
//            this.bgWorker.DoWork += new DoWorkEventHandler(bgWorker_Dowork);
//            this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

//        }

//        public void DoUpdate()
//        {
//            if (!this.bgWorker.IsBusy)
//                this.bgWorker.RunWorkerAsync(this.applicationInfo);
//        }

//        private void bgWorker_Dowork(object sender, DoWorkEventArgs e)
//        {
//            ISharpUpdatable application = (ISharpUpdatable)e.Argument;

//            if (!SharpUpdateXml.ExistsOnServer(application.UpdateXmlLocation))
//                e.Cancel = true;
//            else
//            {
                
//                e.Result = SharpUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationId);
//            }

//        }

//        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            if (!e.Cancelled)
//            {
//                SharpUpdateXml update = (SharpUpdateXml)e.Result;
//                var ye = update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version);

//                if (update != null && update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version))
//                {
//                    if (new SharpUpdateAcceptForm(this.applicationInfo, update).ShowDialog(this.applicationInfo.Context) == DialogResult.Yes)
//                    {
//                        this.DownloadUpdate(update);
//                    }
//                }
//            }

//        }

//        private void DownloadUpdate(SharpUpdateXml update)
//        {
//            SharpUpdateDownloadForm from = new SharpUpdateDownloadForm(update.Uri, update.MD5, this.applicationInfo.ApplicationIcon);

//            DialogResult result = from.ShowDialog(this.applicationInfo.Context);

//            if (result == DialogResult.OK)
//            {
//                string currentPath = this.applicationInfo.ApplicationAssembly.Location;
//                string newPath = Path.GetDirectoryName(currentPath) + "\\" + update.FileName;

//                UpdateApplication(from.TempFilePath, currentPath, newPath, update.LaunchArgs);

//                Application.Exit();
//            }
//            else if (result == DialogResult.Abort)
//            {
//                MessageBox.Show(" The update download was cancelled. \nThis program hasw not been modified.", "Update Download Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            else
//            {
//                MessageBox.Show("There was a problem downloading the update .\nPlease try again later.", "Update Download Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
//        {
//            string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

//            ProcessStartInfo info = new ProcessStartInfo();
//            info.Arguments = String.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs);
//            info.WindowStyle = ProcessWindowStyle.Hidden;
//            info.CreateNoWindow = true;
//            info.FileName = "cmd.exe";
//            Process.Start(info);
//        }

//        public void RollBack()
//        {
//            if (downloadFileListTemp != null)
//            {
//                foreach (DownloadFileInfo file in downloadFileListTemp)
//                {
//                    string tempUrlPath = CommonUtility.GetFolderUrls(file);
//                    string oldPath = string.Empty;
//                    try
//                    {
//                        if (!string.IsNullOrEmpty(tempUrlPath))
//                        {
//                            oldPath = Path.Combine(CommonUtility.SystemBinUrl + CommonUnitity.SystemBinUrl, file.FileName);
//                        }
//                        else
//                        {
//                            oldPath = Path.Combine(CommonUnitity.SystemBinUrl, file.FileName);
//                        }

//                        if (oldPath.EndsWith("_"))
//                            oldPath = oldPath.Substring(0, oldPath.Length - 1);

//                        MoveFolderToOld(oldPath + ".old", oldPath);

//                    }
//                    catch (Exception ex)
//                    {
//                        //log the error message,you can use the application's log code
//                    }
//                }
//            }
//        }



//        private void MoveFolderToOld(string oldPath, string newPath)
//        {
//            if (File.Exists(oldPath) && File.Exists(newPath))
//            {
//                System.IO.File.Copy(oldPath, newPath, true);
//            }
//        }


//        private static void UpdateApplicationsAndMoveToOld(string tempFilePath, string currentPath, string newPath, string oldPath)
//        {
//            string argument = "/C Choice /C Y /N /D Y /T 4 & Move /Y \"{0}\" \"{1}\" & Choice /C Y /N /D Y /T 4 & Move /Y \"{2}\" \"{3}\"  ";

//            ProcessStartInfo info = new ProcessStartInfo();
//            info.Arguments = String.Format(argument, currentPath, oldPath, tempFilePath, newPath);
//            info.WindowStyle = ProcessWindowStyle.Hidden;
//            info.CreateNoWindow = true;
//            info.FileName = "cmd.exe";
//            Process.Start(info);
//        }


//        private static void CreateZipFile(string ZipedFileFolder, string SourceFolder)
//        {
//            // Create and open a new ZIP file
//            string zipFileName = string.Format("zipfile-{0:yyyy-MM-dd_hh-mm-ss-tt}.zip", DateTime.Now);
//            string zipFilepath = System.IO.Path.Combine(ZipedFileFolder, zipFileName);
//            var zip = ZipFile.Open(zipFilepath, ZipArchiveMode.Create);
//            string[] filesToZip = Directory.GetFiles(SourceFolder, "*.txt", SearchOption.AllDirectories);
//            foreach (var file in filesToZip)
//            {
//                // Add the entry for each file
//                zip.CreateEntryFromFile(file, System.IO.Path.GetFileName(file), CompressionLevel.Optimal);
//            }
//            // Dispose of the object when we are done
//            zip.Dispose();
//        }


//        private static void UnZipFile(string ZipedFileFolder, string extractPath)
//        {
//            //how to unzip a existing zip file to a folder
//            string zipPath = Directory.GetFiles(ZipedFileFolder, "*.zip", SearchOption.AllDirectories).FirstOrDefault();
//            //string extractPath = @"E:\Extract";

//            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
//        }



//        private async Task UnInstallClient()
//        {
//            try
//            {
//                var update = false;
//                //var version = (object)null;
//                var version = "";
//                //query to get all installed products
//                var ProgramName = "Emageia.Workshiftly.MainApplication";
//                // var sMSIPath = "";

//                //load the query string
//                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
//                //get the specified proram(s)
//                foreach (ManagementObject obj in mos.Get())
//                {
//                    try
//                    {
//                        //make sure that we are uninstalling the correct application
//                        if (obj["Name"].ToString() == ProgramName)
//                        {
//                            version = obj["Version"].ToString();
//                            //call to Win32_Product Uninstall method, no parameters needed
//                            object hr = obj.InvokeMethod("Uninstall", null);
//                            // return (bool)hr;
//                            update = true;


//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        //this program may not have a name property, so an exception will be thrown
//                    }
//                }

//                if (update)
//                {
//                    if (ExistsOnServer(new Uri("https://localhost:44300/Api//updates/update-available")))
//                    {
//                        try
//                        {
//                            var returnData = new ServerReturnUrlObject();
//                            using (HttpClient client = new HttpClient())
//                            {


//                                HttpResponseMessage response = client.GetAsync(new Uri("https://localhost:44300/Api//updates/update-available")).Result;


//                                if (response.IsSuccessStatusCode)
//                                {
//                                    var returnUrls = await response.Content.ReadAsStringAsync();
//                                    returnData = JsonConvert.DeserializeObject<ServerReturnUrlObject>(returnUrls);

//                                    try
//                                    {
//                                        if (returnData.error && (Int32.Parse(version.ToString()) < Int32.Parse(returnData.version)))
//                                        {
//                                            var Install = await Task.Run(() => RunDownloadInstallMSI(new Uri(returnData.url), returnData.version));
                                           

//                                        }
                                       

//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        CommonUtility.LogWriteLine("Error client download");
//                                    }
//                                }
//                                else
//                                {

//                                }
//                            }





//                        }
//                        catch (Exception ex)
//                        {

//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public static bool ExistsOnServer(Uri loacation)
//        {
//            try
//            {
//                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(loacation.AbsoluteUri);
//                HttpWebResponse resq = (HttpWebResponse)req.GetResponse();
//                resq.Close();

//                return resq.StatusCode == HttpStatusCode.OK;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }

//        public async static Task<bool> RunDownloadInstallMSI(Uri url, string version)
//        {
//            try
//            {
//                var _httpClient = new HttpClient();
//                //var response = await client.GetAsync(@"http://localhost:9000/api/file/GetFile?filename=myPackage.zip");
//                var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\Emageia.Workshiftly.MainApplication-{version}.msi";

//                byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);
//                File.WriteAllBytes(sMSIPath, fileBytes);


//                return true;
//            }
//            catch
//            {
//                CommonUtility.LogWriteLine("There was a problem installing the application!");
//                //Return False if process ended unsuccessfully
//                return false;
//            }
//        }
//    }
//}
