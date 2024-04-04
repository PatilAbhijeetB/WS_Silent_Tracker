using Emageia.Workshiftly.ProcessExtensionsService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Timers = System.Timers;
namespace Emageia.Workshiftly.MainApplication
{
    partial class TimeTrackerService : ServiceBase
    {
        Timers.Timer timer1 = new Timers.Timer();
        public TimeTrackerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                timer1.Enabled = true;
                timer1.Elapsed += new Timers.ElapsedEventHandler(timer1_Elapsed);
                timer1.Interval = 10000;
                timer1.Start();
            }
            catch (Exception exp)
            {
               
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public void OnDebug()
        {
            OnStart(null);
        }
        private void timer1_Elapsed(object sender, EventArgs e)
        {
            try
            {

                string[] lines = new string[] { DateTime.Now.ToString() + "\t" + "Screenshot taken" };
                File.AppendAllLines(@"C:\TimeTracker\NewLogger.txt", lines);
                if (!AnotherInstanceExists())
                {

                    string str = @"C:\Users\Niroshan\Desktop\My\wpf\WpfBasics\WpfBasics\bin\Debug\WpfBasics.exe";
                    
                    ProcessExtensions.StartProcessAsCurrentUser("WpfApp1.exe");
                    //ActivityLogWriter.LogWrite("Forex App is already running.Only one instance allowed to run", true, "User", Globals.kioskID, Globals.PassportNo);
                    // MessageBox.Show("Forex App is already running.", "Only one instance allowed to run", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //  return;
                }
                string[] lin = new string[] { DateTime.Now.ToString() + "\t" + "Forex App is already running.", "Only one instance allowed to run" };
                File.AppendAllLines(@"C:\TimeTracker\NewLogger.txt", lin);
            }
            catch (Exception exp)
            {
                string[] linesxee = new string[] { DateTime.Now.ToString() + "\t" + "OnStarted Service Stopped" + exp.Message };


                string[] linesx = new string[] { DateTime.Now.ToString() + "\t" + "timer1_Elapsed Stoppe" };
                File.AppendAllLines(@"C:\TimeTracker\NewError.txt", linesx);

            }
        }
        public static bool AnotherInstanceExists()
        {
            Process _currentRunningProcess = Process.GetCurrentProcess();
            Process[] _listOfProcs = Process.GetProcessesByName("WpfBasics");
            if (_listOfProcs.Length > 0)
            {
                return true;
            }
           
            return false;
        }
    }
}
