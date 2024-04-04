using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.ServiceProcess;

namespace Emageia.Workshiftly.MainApplication
{
    [RunInstaller(true)]
    public partial class InstallerClass : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;
        public InstallerClass() : base()
        {
            processInstaller = new ServiceProcessInstaller();
           // serviceInstaller = new ServiceInstaller();
          
            processInstaller.Account = ServiceAccount.LocalSystem;

           //serviceInstaller.StartType = ServiceStartMode.Automatic;
           //serviceInstaller.ServiceName = "WorkShiftly Watcher";

         //  Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
            InitializeComponent();
            //this.Committed += new InstallEventHandler(MyInstaller_Committed);
          //  this.OnBeforeUninstall += new UninstallAction(MyUninstallerOnBeforeUninstall);
        }

        // Event handler for 'Committed' event.
        private void MyInstaller_Committed(object sender, InstallEventArgs e)
        {
            try
            {
               
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Emageia.Workshiftly.MainApplication.exe");
                //  Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\workshiftly-desktop-client-1.4.6.exe");
            }
            catch(Exception ex)
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "auto run application Error" + ex.Message.ToString() };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
            }
        }

        private void MyUninstallerOnBeforeUninstall()
        {
            try
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "My Uninstaller On Before Uninstaller"  };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);


              //  var ProgramName = "workshiftly-desktop-client";
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
                            //call to Win32_Product Uninstall method, no parameters needed
                            object hr = mo.InvokeMethod("Uninstall", null);
                            // return (bool)hr;
                        }
                    }
                    catch (Exception ex)
                    {
                        //this program may not have a name property, so an exception will be thrown
                    }
                }

                // Replace "YourServiceName" with the actual name of the service you want to uninstall
                string serviceName = "WorkShiftly Watcher";

                // Stop the service before uninstalling it (optional)
                StopService(serviceName);

                // Uninstall the service
                UninstallService(serviceName);
                //was not found...
                // return false;
            }
            catch (Exception ex)
            {
                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "My Uninstaller On Before Uninstaller" + ex.Message.ToString() };
                File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
            }
        }

        // Override the 'Install' method.
        public override void Install(IDictionary savedState)
        {
            base.Install(savedState);
        }
        // Override the 'Commit' method.
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }
        // Override the 'Rollback' method.
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {

            //try
            //{
            //    var ProgramName = "workshiftly-desktop-client";
            //    //load the query string
            //    ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
            //    //get the specified proram(s)
            //    foreach (ManagementObject mo in mos.Get())
            //    {
            //        try
            //        {
            //            //make sure that we are uninstalling the correct application
            //            if (mo["Name"].ToString() == ProgramName)
            //            {
            //                //call to Win32_Product Uninstall method, no parameters needed
            //                object hr = mo.InvokeMethod("Uninstall", null);
            //                // return (bool)hr;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            //this program may not have a name property, so an exception will be thrown
            //        }
            //    }
            //    //was not found...
            //    // return false;
            //}
            //catch (Exception ex)
            //{
            //    // return false;
            //}
            //finally
            //{
            //    base.Uninstall(savedState);
            //}
           
            base.Uninstall(savedState);
            this.MyUninstallerOnBeforeUninstall();

        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            Context.Parameters["assemblypath"] = "\"" +
                Context.Parameters["assemblypath"] + "\" -someWork";
            base.OnBeforeInstall(savedState);

        }
        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            Context.Parameters["assemblypath"] = "\"" +
                Context.Parameters["assemblypath"] + "\" -service";
            base.OnBeforeUninstall(savedState);
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
                                reg.DeleteValue("WorkShiftly", false);
                            }

                        }
                        else
                        {
                            // MessageBox.Show("key not found");
                        }


                }
                catch (Exception ex)
                {
                    string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "AutoRunWindowsStartup" + ex.Message.ToString() };
                    File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", lines);
                }

               
            
            
        }

        static void StopService(string serviceName)
        {
            using (ServiceController serviceController = new ServiceController(serviceName))
            {
                if (serviceController.Status == ServiceControllerStatus.Running)
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }
            }
        }

        static void UninstallService(string serviceName)
        {
            string path = $"sc delete {serviceName}";

            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/c {path}";
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;

                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
        }
    }
}
