/*****************************************************************

 * Author:   Sameera Niroshan 
 * Email:    sameera@emagia.com
 * Create Date:  18/02/2022 
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Emageia.Workshiftly.AutoUpdater.WindowFoms;
using System.Collections.Generic;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using System.IO.Compression;
using System.Linq;

namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper
{
    public class SharpUpdater
    {
        private ISharpUpdatable applicationInfo;
        private BackgroundWorker bgWorker;
        List<DownloadFileInfo> downloadFileListTemp = null;
        private bool bNeedRestart = false;
        private bool bDownload = false;
        public SharpUpdater(ISharpUpdatable applicationInfo)
        {
            this.applicationInfo = applicationInfo;
            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += new DoWorkEventHandler(bgWorker_Dowork);
            this.bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

        }

        public void DoUpdate()
        {
            if (!this.bgWorker.IsBusy)
                this.bgWorker.RunWorkerAsync(this.applicationInfo);
        }

        private void bgWorker_Dowork(object sender, DoWorkEventArgs e)
        {
            ISharpUpdatable application = (ISharpUpdatable)e.Argument;

            if (!SharpUpdateXml.ExistsOnServer(application.UpdateXmlLocation))
                e.Cancel = true;
            else
            {
                //var ne = SharpUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationId);
                e.Result = SharpUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationId);
            }

        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                SharpUpdateXml update = (SharpUpdateXml)e.Result;
                var ye = update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version);

                if (update != null && update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version))
                {
                    if (new SharpUpdateAcceptForm(this.applicationInfo, update).ShowDialog(this.applicationInfo.Context) == DialogResult.Yes)
                    {
                        this.DownloadUpdate(update);
                    }
                }
            }

        }

        private void DownloadUpdate(SharpUpdateXml update)
        {
            SharpUpdateDownloadForm from = new SharpUpdateDownloadForm(update.Uri, update.MD5, this.applicationInfo.ApplicationIcon);

            DialogResult result = from.ShowDialog(this.applicationInfo.Context);

            if (result == DialogResult.OK)
            {
                string currentPath = this.applicationInfo.ApplicationAssembly.Location;
                string newPath = Path.GetDirectoryName(currentPath) + "\\" + update.FileName;

                UpdateApplication(from.TempFilePath, currentPath, newPath, update.LaunchArgs);

                Application.Exit();
            }
            else if (result == DialogResult.Abort)
            {
                MessageBox.Show(" The update download was cancelled. \nThis program hasw not been modified.", "Update Download Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("There was a problem downloading the update .\nPlease try again later.", "Update Download Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


        private static void UnZipFile(string ZipedFileFolder, string extractPath)
        {
            //how to unzip a existing zip file to a folder
            string zipPath = Directory.GetFiles(ZipedFileFolder, "*.zip", SearchOption.AllDirectories).FirstOrDefault();
            //string extractPath = @"E:\Extract";

            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
        }
    }
}
