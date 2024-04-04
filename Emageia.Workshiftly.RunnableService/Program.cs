using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.RunnableService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RunnableService()
            };
            ServiceBase.Run(ServicesToRun);

            //foreach (string arg in Environment.GetCommandLineArgs())
            //{
            //    Console.WriteLine(arg);
            //    string[] linesc = new string[] { DateTime.Now.ToString() + "\t" + arg };
            //    File.AppendAllLines(@"E:\mainServiceBAse .txt", linesc);
            //    MyUninstallerOnBeforeUninstall();
        
            //}
            //Console.ReadLine();
            //RunnableService timeRunner = new RunnableService();
            //timeRunner.OnDebug();
            //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);


        }

        private static void MyUninstallerOnBeforeUninstall()
        {
            try
            {

                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "MyUninstallerOnBeforeUninstall" };
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
                            string[] linesc = new string[] { DateTime.Now.ToString() + "\t" + "UnInstallDeployApplications function mo.InvokeMethod 11111111111"+ nadw };
                            File.AppendAllLines(@"D:\WorkshiftlyTimetracker.txt", linesc);

                            object hr = mo.InvokeMethod("Uninstall", null);

                            //  UnInstallDeployApplications(mo["IdentifyingNumber"].ToString());
                            string[] liness = new string[] { DateTime.Now.ToString() + "\t" + "UnInstallDeployApplications function 3333333333" };
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

    }
}
