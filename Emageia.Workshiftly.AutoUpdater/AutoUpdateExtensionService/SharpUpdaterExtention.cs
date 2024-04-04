using Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using Emageia.Workshiftly.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateExtensionService
{
    public class SharpUpdaterExtention
    {
        private ISharpUpdatable applicationInfo;
        private BackgroundWorker bgWorker;
        List<DownloadFileInfo> downloadFileListTemp = null;
        private bool bNeedRestart = false;
        private bool bDownload = false;

        private static string SystemBinUrl = AppDomain.CurrentDomain.BaseDirectory;

        public SharpUpdaterExtention(ISharpUpdatable applicationInfo)
        {
            this.applicationInfo = applicationInfo;
            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += new DoWorkEventHandler(bgWorker_Dowork);
            this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
        }

      
        public void DoUpdate()
        {
            SetEnvironmentVariableForUpdateWpf(true, false);
            if (!this.bgWorker.IsBusy)
                this.bgWorker.RunWorkerAsync(this.applicationInfo);
        }
        private void bgWorker_Dowork(object sender, DoWorkEventArgs e)
        {
            ISharpUpdatable application = (ISharpUpdatable)e.Argument;

            if (!SharpUpdateXml.ExistsOnServer(application.UpdateXmlLocation))
            {
                e.Cancel = true;
                SetEnvironmentVariableForUpdateWpf(false, false);
            }
            else
            {
              //  var ne = SharpUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationId);
                e.Result = SharpUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationId);
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                SharpUpdateXml update = (SharpUpdateXml)e.Result;

                this.downloadFileListTemp = update.downloadFileListTemp;
                //var ye = update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version);

                if (update != null && update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version))
                {
                    //if (new SharpUpdateAcceptForm(this.applicationInfo, update).ShowDialog(this.applicationInfo.Context) == DialogResult.Yes)
                    //{
                    DownloadUpdate(update);
                    //}
                }
                else
                {
                    SetEnvironmentVariableForUpdateWpf(false, false);
                }
            }
        }

        public const string RESOURCEFOLDERNAME = @"Resource\CSharpClient";
        public const string CLIENTFOLDERNAME = @"Appliction\Client";


        public const string CLIENTUPDATEDONLOADFOLDER = @"Appliction\Client\download";
        public const string ClintNewUpdateDownloadFOLDER = @"Appliction\Client";

        public const string ClintoldDownloadFOLDER = @"Appliction\Client\OldFiles";


        public const string RESOURCECLIENTFOLDERNAME = @"Appliction\Client\Release";
        public const string CLIENTFILEINSTALLERNAME = @"Appliction\Client\Release\Emageia.Workshiftly.MainApplication.exe";

        #region  file download and replace
        //private async void DownloadUpdate(SharpUpdateXml update)
        //{


        //   // if (SharpUpdateXml.ExistsOnServer(new Uri("https://localhost:44300/Api//updates/update-available")))
        //    if (SharpUpdateXml.ExistsOnServer(new Uri("https://localhost:8000/Api//updates/update-available")))
        //    {
        //        try
        //        {
        //            var downFilePath = SystemBinUrl + CLIENTUPDATEDONLOADFOLDER;
        //            if (!Directory.Exists(SystemBinUrl + ClintNewUpdateDownloadFOLDER))
        //            {
        //                Directory.CreateDirectory(SystemBinUrl + ClintNewUpdateDownloadFOLDER); 
        //            }
        //            if (!Directory.Exists(downFilePath))
        //            {
        //                Directory.CreateDirectory(downFilePath);    
        //            }
        //            if (!Directory.Exists(SystemBinUrl + ClintoldDownloadFOLDER))
        //            {
        //                Directory.CreateDirectory(SystemBinUrl + ClintoldDownloadFOLDER);
        //            }

        //            var _httpClient = new HttpClient();
        //            //var response = await client.GetAsync(@"http://localhost:9000/api/file/GetFile?filename=myPackage.zip");
        //            var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\workshiftly-desktop-client.zip";

        //            byte[] fileBytes = await _httpClient.GetByteArrayAsync(update.ZipFileDownloadUri);
        //            File.WriteAllBytes(downFilePath+@"\Update.zip", fileBytes);

        //            //hashing 
        //            if (Hasher.HashFile(downFilePath + @"\Update.zip", HashType.MD5) ==   update.DownloadZipMD5)
        //            {
        //                deleteDirectoryCount(SystemBinUrl + ClintNewUpdateDownloadFOLDER + @"\Update");
        //                UnZipFile(downFilePath, SystemBinUrl + ClintNewUpdateDownloadFOLDER);

        //                movingFileToOldFolder();

        //                UpdatedFileMovetotheCurrentFolder();
        //            }


        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //}

        #endregion


       // private async Task<(bool,string)> DownloadUpdate(SharpUpdateXml update)
        private async void DownloadUpdate(SharpUpdateXml update)
        {


            // if (SharpUpdateXml.ExistsOnServer(new Uri("https://localhost:44300/Api//updates/update-available")))
            if (SharpUpdateXml.ExistsOnServer(new Uri("https://localhost:8000/Api//updates/update-available")))
            {
                try 
                {
                    var getDetails = GetEnvironmentVariableForUpdateWpf();
                    var downFilePath = SystemBinUrl + CLIENTUPDATEDONLOADFOLDER;
                    if (!Directory.Exists(SystemBinUrl + ClintNewUpdateDownloadFOLDER))
                    {
                        Directory.CreateDirectory(SystemBinUrl + ClintNewUpdateDownloadFOLDER);
                    }
                    if (!Directory.Exists(downFilePath))
                    {
                        Directory.CreateDirectory(downFilePath);
                    }
                    if (!Directory.Exists(SystemBinUrl + ClintoldDownloadFOLDER))
                    {
                        Directory.CreateDirectory(SystemBinUrl + ClintoldDownloadFOLDER);
                    }

                    var _httpClient = new HttpClient();
                    //var response = await client.GetAsync(@"http://localhost:9000/api/file/GetFile?filename=myPackage.zip");
                   // var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\workshiftly-desktop-client.zip";
                    var sMSIPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\Resource\\WorkShiftly.zip";
                    sMSIPath = Path.Combine(CommonUtility.SubFolderPath + $"\\Release.zip");

                    if (File.Exists(sMSIPath))
                    {
                        File.Delete(sMSIPath);
                        byte[] fileBytes = await _httpClient.GetByteArrayAsync(update.ZipFileDownloadUri);
                        File.WriteAllBytes(sMSIPath, fileBytes);
                    }
                       
                    //hashing 
                    if (Hasher.HashFile(sMSIPath, HashType.MD5) == update.DownloadZipMD5)
                    {
                        
                        MoveFolderToOld(sMSIPath, getDetails.storedPath);
                        SetEnvironmentVariableForUpdateWpf(false, true);
                        //  return (true, sMSIPath);
                    }

                    SetEnvironmentVariableForUpdateWpf(false, false);
                    //  return (false, sMSIPath);
                }
                catch (Exception ex)
                {
                    SetEnvironmentVariableForUpdateWpf(false, false);
                    //  return (false, null);
                }
            }

           // return (false, null);
        }

        private void movingZipFileToServiceFolder()
        {
            try
            {
                string fileName = "test.txt";
                string sourcePath = SystemBinUrl + ClintNewUpdateDownloadFOLDER;
                string targetPath = SystemBinUrl + ClintoldDownloadFOLDER;

                deleteDirectoryCount(targetPath);

                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileName);


                if (System.IO.Directory.Exists(SystemBinUrl + CLIENTUPDATEDONLOADFOLDER))
                {
                    string[] files = System.IO.Directory.GetFiles(SystemBinUrl + ClintNewUpdateDownloadFOLDER + @"\Update");

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(targetPath, fileName);

                        var filesNames = downloadFileListTemp.FirstOrDefault(x => x.FileName == fileName);
                        if (filesNames != null)
                        {
                            System.IO.File.Copy(s, destFile, true);
                        }

                    }
                }
                else
                {
                    Console.WriteLine("Source path does not exist!");
                }
            }
            catch (Exception)
            {

            }

        }
        private void movingFileToOldFolder()
        {
            try
            {
                string fileName = "test.txt";
                string sourcePath = SystemBinUrl + ClintNewUpdateDownloadFOLDER;
                string targetPath = SystemBinUrl + ClintoldDownloadFOLDER;

                deleteDirectoryCount(targetPath);

                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileName);


                if (System.IO.Directory.Exists(SystemBinUrl + CLIENTUPDATEDONLOADFOLDER))
                {
                    string[] files = System.IO.Directory.GetFiles(SystemBinUrl + ClintNewUpdateDownloadFOLDER + @"\Update");

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(targetPath, fileName);

                        var filesNames = downloadFileListTemp.FirstOrDefault(x => x.FileName == fileName);
                        if (filesNames != null)
                        {
                            System.IO.File.Copy(s, destFile, true);
                        }

                    }
                }
                else
                {
                    Console.WriteLine("Source path does not exist!");
                }
            }
            catch (Exception)
            {

            }
            
        }

        private void UpdatedFileMovetotheCurrentFolder()
        {
            try
            {
                string fileName = "test.txt";
                string sourcePath = SystemBinUrl + ClintNewUpdateDownloadFOLDER + @"\Update";
                string targetPath = SystemBinUrl + RESOURCECLIENTFOLDERNAME;

                deleteDirectoryCount(sourcePath);

                string sourceFile = Path.Combine(sourcePath, fileName);
                string destFile = Path.Combine(targetPath, fileName);


                if (Directory.Exists(sourcePath))
                {
                    string[] files = Directory.GetFiles(sourcePath);
                    string mainFileName = "Emageia.Workshiftly.MainApplication.exe";
                    string mainFilepath = string.Empty;

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        if (Path.GetFileName(s) != mainFileName)
                        {
                            fileName = Path.GetFileName(s);
                            destFile = Path.Combine(targetPath, fileName);
                        }
                        else
                        {
                            mainFilepath = s;
                        }
                    }


                }
                else
                {
                    Console.WriteLine("Source path does not exist!");
                }
            }
            catch (Exception)
            {

               
            }
            
        }

        private void deleteDirectoryCount(string sourcePath)
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

        public void RollBack()
        {
            if (downloadFileListTemp != null)
            {
                foreach (DownloadFileInfo file in downloadFileListTemp)
                {
                    string tempUrlPath = CommonUnitity.GetFolderUrls(file);
                    string oldPath = string.Empty;
                    try
                    {
                        if (!string.IsNullOrEmpty(tempUrlPath))
                        {
                            oldPath = Path.Combine(CommonUtility.SystemBinUrl + CommonUnitity.SystemBinUrl, file.FileName);
                        }
                        else
                        {
                            oldPath = Path.Combine(CommonUnitity.SystemBinUrl, file.FileName);
                        }

                        if (oldPath.EndsWith("_"))
                            oldPath = oldPath.Substring(0, oldPath.Length - 1);

                        MoveFolderToOld(oldPath + ".old", oldPath);

                    }
                    catch (Exception ex)
                    {
                        //log the error message,you can use the application's log code
                    }
                }
            }
        }



        private void MoveFolderToOld(string oldPath, string newPath)
        {
            if (File.Exists(oldPath) && File.Exists(newPath))
            {
                System.IO.File.Copy(oldPath, newPath, true);
            }
        }


        private static void UpdateApplicationsAndMoveToOld(string tempFilePath, string currentPath, string newPath, string oldPath)
        {
            string argument = "/C Choice /C Y /N /D Y /T 4 & Move /Y \"{0}\" \"{1}\" & Choice /C Y /N /D Y /T 4 & Move /Y \"{2}\" \"{3}\"  ";

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = String.Format(argument, currentPath, oldPath, tempFilePath, newPath);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }


        private static void UnZipFile(string ZipedFileFolder, string extractPath)
        {
            try
            {
                //how to unzip a existing zip file to a folder
                string zipPath = Directory.GetFiles(ZipedFileFolder, "*.zip", SearchOption.AllDirectories).FirstOrDefault();
                //string extractPath = @"E:\Extract";

                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (Exception)
            {

                
            }
          
        }
        #region Encryption and Decryption


        public static void SetEnvironmentVariableForUpdateWpf(bool updatingWpf, bool SuccessfulyDownload)
        {
            
            var getDetails = GetEnvironmentVariableForUpdateWpf();
            string user = nameof(user);

            var newMSIPath = Path.Combine(CommonUtility.SubFolderPath + $"\\Release.zip");

            if (getDetails != null)
            {
                jsonEncrypt(false, false, false, newMSIPath,"", false, user);
                              
            }
            else if (!getDetails.UpdatingWpf && !getDetails.UpdatingServer && updatingWpf)
            {
                jsonEncrypt(updatingWpf, getDetails.UpdatingServer, getDetails.SuccessfulyDownload, newMSIPath, getDetails.storedPath, getDetails.Extract, user);
   
            }
            else if(getDetails.UpdatingWpf && !getDetails.SuccessfulyDownload && SuccessfulyDownload)
            {
                jsonEncrypt(false, getDetails.UpdatingServer, SuccessfulyDownload, newMSIPath, getDetails.storedPath, getDetails.Extract, user);
            }



            

        }

        //public void SetEnvironmentVariableForUpdateWpf(bool updatingWpf, bool SuccessfulyDownload)
        //{

        //    var getDetails = GetEnvironmentVariableForUpdateWpf();
        //    string user = nameof(user);

        //    var newMSIPath = Path.Combine(CommonUtility.SubFolderPath + $"\\Release.zip");

        //    if (getDetails != null)
        //    {
        //        jsonEncrypt(false, false, false, newMSIPath, getDetails.storedPath, getDetails.Extract, user);

        //    }
        //    else if (!getDetails.UpdatingWpf && !getDetails.UpdatingServer && updatingWpf)
        //    {
        //        jsonEncrypt(updatingWpf, getDetails.UpdatingServer, getDetails.SuccessfulyDownload, newMSIPath, getDetails.storedPath, getDetails.Extract, user);

        //    }
        //    else if (getDetails.UpdatingWpf && !getDetails.SuccessfulyDownload && SuccessfulyDownload)
        //    {
        //        jsonEncrypt(false, getDetails.UpdatingServer, SuccessfulyDownload, newMSIPath, getDetails.storedPath, getDetails.Extract, user);
        //    }





        //}

        private static void jsonEncrypt(bool updatingWpf,bool updatingServer,bool SuccessfulyDownload,string downloadpath,string storedPath, bool extract, string user)
        {
            try
            {
                SyncUpdateStatus updateStatus = new SyncUpdateStatus()
                {
                    Id = 1,
                    UpdatingWpf = updatingWpf,
                    UpdatingServer = updatingServer,
                    SuccessfulyDownload = SuccessfulyDownload,
                    downloadpath = downloadpath,
                    Extract = extract,
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
    }
}
