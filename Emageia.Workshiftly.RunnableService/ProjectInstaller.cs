using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.RunnableService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        //private ServiceInstaller serviceInstaller;
        //private ServiceProcessInstaller processInstaller;
        public ProjectInstaller()
        {

            //System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(serviceInstaller.ServiceName);
            //if (controller.Status == ServiceControllerStatus.Running | controller.Status == ServiceControllerStatus.Paused)
            //{
            //    controller.Stop();
            //}


            //controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));
            //controller.Close();

            //processInstaller = new ServiceProcessInstaller();
            //serviceInstaller = new ServiceInstaller();

            //processInstaller.Account = ServiceAccount.LocalSystem;

            //serviceInstaller.StartType = ServiceStartMode.Automatic;
            //serviceInstaller.ServiceName = "Emageia Runnable Service";

            //Installers.Add(serviceInstaller);
            //Installers.Add(processInstaller);
            InitializeComponent();
        }



        private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller.ServiceName).Start();
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(serviceInstaller.ServiceName);

            try
            {
                string[] lines4 = new string[] {"==============="+ "\t" + "OnBeforeUninstall running" };
                File.AppendAllLines(@"C:\OnBeforeUninstall.txt", lines4);

                if (controller.Status == ServiceControllerStatus.Running | controller.Status == ServiceControllerStatus.Paused)
                {
                    controller.Stop();
                }


                controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));
                controller.Close();



                Process[] workers = Process.GetProcessesByName("Emageia.Workshiftly.MainApplication");
                foreach (Process worker in workers) 
                {
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                //string[] lines5 = new string[] { DateTime.Now +"===============" + "\t" + "After Killing running" };
                //File.AppendAllLines(@"C:\OnBeforeUninstall.txt", lines5);

                // string sourcePath = @"C:\Program Files (x86)\Default Company Name\WorkshiftlyWindowsService\Appliction";
                string sourcePath = @"C:\Program Files (x86)\WorkShiftlyClient\WorkshiftlyWindowsService\Appliction";
                // deleteFiles
                if (Directory.Exists(sourcePath))
                {
                    deleteDirectoryCount(sourcePath);
                }
               


                var paths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var subFolderPath = System.IO.Path.Combine(paths, "Workshiftly Client");
                if (Directory.Exists(subFolderPath))
                {
                    deleteDirectoryCount(subFolderPath);
                }


               

                //  string[] lines6 = new string[] { DateTime.Now + "===============" + "\t" + "After Delete directory" };
                //  File.AppendAllLines(@"C:\OnBeforeUninstall.txt", lines6);

                //  Thread syncActiveWindow = new Thread(() => MyUninstallerOnBeforeUninstall());
                //  syncActiveWindow.Start();

                //  Thread.SpinWait(10000);
                ////  MyUninstallerOnBeforeUninstall();

                //  string[] lines7 = new string[] { DateTime.Now + "===============" + "\t" + "After MyUninstallerOnBeforeUninstall" };
                //  File.AppendAllLines(@"C:\OnBeforeUninstall.txt", lines7);
            }
            catch (Exception ex)
            {
                EventLog log = new EventLog();
                log.WriteEntry("Service failed to stop");

                string[] lines4 = new string[] { ex.ToString() + "\t" + "OnBeforeUninstall" };
                File.AppendAllLines(@"C:\OnBeforeUninstall.txt", lines4);
            }

            finally
            {
                base.OnBeforeUninstall(savedState);
            }



        }

        private static void deleteDirectoryCount(string sourcePath)
        {
            try
            {
                if (Directory.Exists(sourcePath))
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
            }
            catch (Exception ex)
            {

                EventLog log = new EventLog();
                log.WriteEntry("Service failed to delete Directory");

                string[] lines4 = new string[] { "Service failed to delete Directory" + ex.ToString() + "\t" + "OnBeforeUninstall" };
                File.AppendAllLines(@"C:\OnBeforeUninstall.txt", lines4);
            }



        }
        private void MyInstaller_Committed(object sender, InstallEventArgs e)
        {
            try
            {

                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Emageia.Workshiftly.MainApplication.exe");
                //  Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\workshiftly-desktop-client-1.4.6.exe");
            }
            catch (Exception ex)
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "auto run application Error" + ex.Message.ToString() };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
            }
        }




        private void MyUninstallerOnBeforeUninstall()
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
                            string[] linesc = new string[] { DateTime.Now.ToString() + "\t" + "UnInstallDeployApplications function mo.InvokeMethod 11111111111" };
                            File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", linesc);

                            object hr = mo.InvokeMethod("Uninstall", null,null);
                            
                            //  UnInstallDeployApplications(mo["IdentifyingNumber"].ToString());
                            string[] liness = new string[] { DateTime.Now.ToString() + "\t" + "UnInstallDeployApplications function 3333333333"  };
                            File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", liness);
                        }
                    }
                    catch (Exception ex)
                    {
                        string[] linesx = new string[] { DateTime.Now.ToString() + "\t" + "UnInstallDeployApplications function 2222222222222222" + ex.ToString() };
                        File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", linesx);
                        //this program may not have a name property, so an exception will be thrown
                    }
                }
                
            }
            catch (Exception ex)
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "My Uninstaller On Before Uninstaller  " + ex.Message.ToString() };
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
                // string uuid = "{312FABEA-ADE3-3A2C-9DA6-2292A58501B0} ";
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                process.StartInfo = startInfo;

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardError = true;

                startInfo.FileName = "msiexec.exe";
                startInfo.Arguments = string.Format(UninstallCommandString, uuid);

                process.Start();

            }
            catch (Exception ex)
            {
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


        private async Task UnInstallJavaClient()
        {
            try
            {

                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "My Uninstaller On Before Uninstaller" };
                File.AppendAllLines(@"D:\OnBeforeUninstall.txt", lines);
                var update = false;
                //var version = (object)null;
                var version = "";
                //query to get all installed products
                //var ProgramName = "workshiftly-desktop-client";
                var ProgramName = "WorkShiftly";
                // var ProgramName = "EmageiaService";
                // var sMSIPath = "";

                //load the query string
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                //get the specified proram(s)
                foreach (ManagementObject obj in mos.Get())
                {
                    try
                    {
                        //make sure that we are uninstalling the correct application
                        if (obj["Name"].ToString() == ProgramName)
                        {
                            string[] linessd = new string[] { DateTime.Now.ToString() + "\t" + "ProgramName" + obj["Name"].ToString() };
                            //            File.AppendAllLines(@"E:\OnBeforeUninstall.txt", linessd);
                            version = obj["Version"].ToString();
                            //call to Win32_Product Uninstall method, no parameters needed
                            object hr = obj.InvokeMethod("Uninstall", null);
                            // return (bool)hr;
                            update = true;


                        }
                    }
                    catch (Exception ex)
                    {
                        string[] linessdf = new string[] { DateTime.Now.ToString() + "\t" + "cathd" };
                        File.AppendAllLines(@"D:\OnBeforeUninstall.txt", linessdf);
                        //this program may not have a name property, so an exception will be thrown
                    }
                }


                string[] linesd = new string[] { DateTime.Now.ToString() + "\t" + "End" };
                File.AppendAllLines(@"D:\OnBeforeUninstall.txt", linesd);


            }
            catch (Exception ex)
            {

            }
        }




    }
}
