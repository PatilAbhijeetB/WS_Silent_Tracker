using Emageia.Workshiftly.Entity.ServiceModels;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Timers = System.Timers;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.SystemDetails
{
    public class Utilization
    {
        Timers.Timer timer1 = new Timers.Timer();
        private string machineName = System.Environment.MachineName;
        private int smallLabelWidth = 40;
        private int bigLabelWidth = 60;

        private PerformanceCounter cpuCounter = null;
        private PerformanceCounter ramCounter = null;
        private PerformanceCounter pageCounter = null;
        private PerformanceCounter[] nicCounters = null;

        public Utilization()
        {
            GetMachineName();
            InitCounters();

            //timer1.Enabled = true;
            //timer1.Elapsed += new Timers.ElapsedEventHandler(tTimer_Tick);
            //timer1.Interval = 3000;
            //timer1.Start();
        }

        private void GetMachineName()
        {
            string[] cmdArgs = System.Environment.GetCommandLineArgs();
            if ((cmdArgs != null) && (cmdArgs.Length > 1))
            { this.machineName = cmdArgs[1]; }
        }


        public void InitCounters()
        {
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", machineName);
                //ramCounter = new PerformanceCounter("Memory", "Available MBytes", String.Empty, machineName);
                ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", String.Empty, machineName);
                pageCounter = new PerformanceCounter("Paging File", "% Usage", "_Total", machineName);
                // there can be multiple network interfaces
                nicCounters = GetNICCounters();
                
                // for each nic card, create a corresponding status bar label
                for (int i = 0; i < nicCounters.Length; i++)
                {
                    var ni = GetNICLabel(nicCounters[i], i); 
                }
            }
            catch (Exception ex)
            {
                //Program.HandleException(String.Format("Unable to access computer '{0}'\r\nPlease check spelling and verify this computer is connected to the network", this.machineName));
                //Close();
            }
        }

   

        public UtilizationDetails getInitCouters()
        {
            var loope = true;
            var count = 0;
            UtilizationDetails utilization = new UtilizationDetails();
            while (loope)
            {
                float tmp = cpuCounter.NextValue();

                var CPUProcessorTime = (float)(Math.Round((double)tmp, 1));



                // return the value of Memory Commit Limit
                float tmpramCounter = ramCounter.NextValue();
                var MEMCommitedPerc = (float)(Math.Round((double)tmpramCounter, 1));


                float PageFile = pageCounter.NextValue();
                float nicCounter = 0;

                for (int i = 0; i < nicCounters.Length; i++)
                {
                    //"{0:####0KB/s}"
                     nicCounter = nicCounters[i].NextValue() / 1024;
                    
                }

                count += 1;
                if (CPUProcessorTime > 0 )
                {
                    loope = false;
                    //utilization = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    //{
                    //    cpuUtilization = CPUProcessorTime,
                    //    ramUtilization = MEMCommitedPerc,
                    //    pageUtilization = PageFile,
                    //    networkUtilization = nicCounter,
                    //});
                    CommonFunctions.LogWriteLines("\n utilization ****************************************************** ");

                    utilization = new UtilizationDetails
                    {
                        cpuUtilization = CPUProcessorTime,
                        ramUtilization = MEMCommitedPerc,
                        pageUtilization = PageFile,
                        networkUtilization = nicCounter
                    };

                    CommonFunctions.LogWriteLines("\n cpuUtilization:\t " + CPUProcessorTime + "\t ramUtilization: \t" + MEMCommitedPerc + "\t pageUtilization: \t" + PageFile + "\t networkUtilization: \t" + nicCounter);

                    break;
                }
                else if (count < 4)
                {
                    utilization = new UtilizationDetails();
                }
                else
                {
                    loope = false;
                    break;
                }
                
                Thread.Sleep(3000);
            }

            return utilization;
           
        }



        // ping the remote computer 
        private bool VerifyRemoteMachineStatus(string machineName)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(machineName);
                    if (reply.Status == IPStatus.Success)
                    { return true; }
                }
            }
            catch (Exception ex)
            {
                // return false for any exception encountered
                // we'll probably want to just shut down anyway
            }
            return false;
        }

        // machine can have multiple nic cards (and laptops usually have wifi & wired) 
        // don't want to get into figuring out which one to show, just get all
        // can enumerate network card other ways (System.Net.NetworkInformation) 

        // PerformanceCounters can return string[] of available network interfaces
        private string[] GetNICInstances(string machineName)
        {
            string filter = "MS TCP Loopback interface";
            List<string> nics = new List<string>();
            PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface", machineName);
            if (category.GetInstanceNames() != null)
            {
                foreach (string nic in category.GetInstanceNames())
                {
                    if (!nic.Equals(filter, StringComparison.InvariantCultureIgnoreCase))
                    { nics.Add(nic); }
                }
            }
            return nics.ToArray();
        }

        // create a Performance Counter for each network interface
        private PerformanceCounter[] GetNICCounters()
        {
            string[] nics = GetNICInstances(this.machineName);
            List<PerformanceCounter> nicCounters = new List<PerformanceCounter>();
            foreach (string nicInstance in nics)
            {
                nicCounters.Add(new PerformanceCounter("Network Interface", "Bytes Total/sec", nicInstance, this.machineName));
            }
            return nicCounters.ToArray();
        }

        // create a ToolStripStatusLabel for each network interface
        private ToolStripStatusLabel GetNICLabel(PerformanceCounter counter, int index)
        {
            ToolStripStatusLabel newLabel = new ToolStripStatusLabel();


            return newLabel;
        }

        // adjust form width based on number of statusbar labels
        //private void SetFormWidth()
        //{
        //    //this.Width = (tsCPU.Width + tsRAM.Width + tsPage.Width);
        //    int width = 0;
        //    foreach (ToolStripStatusLabel label in ssStatusBar.Items)
        //    { width += label.Width + label.Padding.Right; }

        //    if (ssStatusBar.Padding.Horizontal > 0)
        //    { width += (ssStatusBar.Padding.Horizontal / 2); }
        //    this.Width = width;
        //}

    }
}
