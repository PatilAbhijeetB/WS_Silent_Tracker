using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.SystemDetails
{
    public class SysParams
    {
        public static string machineName = string.Empty;
        public SysParams()
        {
            machineName = System.Environment.MachineName;
        }
        
        public string NodeName { get; set; }

        public float CPUProcessorTime { get; set; }

        public float CPUPrivilegedTime { get; set; }

        public float CPUInterruptTime { get; set; }

        public float CPUDPCTime { get; set; }

        public float MEMAvailable { get; set; }

        public float MEMCommited { get; set; }

        public float MEMCommitLimit { get; set; }

        public float MEMCommitedPerc { get; set; }

        public float MEMPoolPaged { get; set; }

        public float MEMPoolNonPaged { get; set; }

        public float MEMCached { get; set; }

        public float PageFile { get; set; }

        public float ProcessorQueueLengh { get; set; }

        public float DISCQueueLengh { get; set; }

        public float DISKRead { get; set; }

        public float DISKWrite { get; set; }

        public float DISKAverageTimeRead { get; set; }

        public float DISKAverageTimeWrite { get; set; }

        public float DISKTime { get; set; }

        public float HANDLECountCounter { get; set; }

        public float THREADCount { get; set; }

        public int CONTENTSwitches { get; set; }

        public int SYSTEMCalls { get; set; }

        public float NetTrafficSend { get; set; }

        public float NetTrafficReceive { get; set; }

        public DateTime SamplingTime { get; set; }

        private PerformanceCounter cpuProcessorTime = new PerformanceCounter("Processor", "% Processor Time", "_Total", System.Environment.MachineName);

        private PerformanceCounter cpuPrivilegedTime = new PerformanceCounter("Processor", "% Privileged Time", "_Total", System.Environment.MachineName);

        private PerformanceCounter cpuInterruptTime = new PerformanceCounter("Processor", "% Interrupt Time", "_Total", System.Environment.MachineName);

        private PerformanceCounter cpuDPCTime = new PerformanceCounter("Processor", "% DPC Time", "_Total", System.Environment.MachineName);

        private PerformanceCounter memAvailable = new PerformanceCounter("Memory", "Available MBytes", String.Empty, System.Environment.MachineName);

        private PerformanceCounter memCommited = new PerformanceCounter("Memory", "Committed Bytes", String.Empty, System.Environment.MachineName);

        private PerformanceCounter memCommitLimit = new PerformanceCounter("Memory", "Commit Limit", String.Empty, System.Environment.MachineName);

        //private PerformanceCounter memCommitedPerc = new PerformanceCounter("Memory", "% Committed Bytes In Use", null);
        private PerformanceCounter memCommitedPerc = new PerformanceCounter("Memory", "% Committed Bytes In Use" ,String.Empty, System.Environment.MachineName);

        private PerformanceCounter memPollPaged = new PerformanceCounter("Memory", "Pool Paged Bytes", String.Empty, System.Environment.MachineName);

        private PerformanceCounter memPollNonPaged = new PerformanceCounter("Memory", "Pool Nonpaged Bytes", String.Empty, System.Environment.MachineName);

        private PerformanceCounter memCached = new PerformanceCounter("Memory", "Cache Bytes", String.Empty, System.Environment.MachineName);

        private PerformanceCounter pageFile = new PerformanceCounter("Paging File", "% Usage", "_Total", System.Environment.MachineName);

        private PerformanceCounter processorQueueLengh = new PerformanceCounter("System", "Processor Queue Length", null);

        private PerformanceCounter diskQueueLengh = new PerformanceCounter("PhysicalDisk", "Avg. Disk Queue Length", "_Total");

        private PerformanceCounter diskRead = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total", System.Environment.MachineName);

        private PerformanceCounter diskWrite = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total", System.Environment.MachineName);

        private PerformanceCounter diskAverageTimeRead = new PerformanceCounter("PhysicalDisk", "Avg. Disk sec/Read", "_Total", System.Environment.MachineName);

        private PerformanceCounter diskAverageTimeWrite = new PerformanceCounter("PhysicalDisk", "Avg. Disk sec/Write", "_Total", System.Environment.MachineName);

        private PerformanceCounter diskTime = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total", System.Environment.MachineName);

        private PerformanceCounter handleCountCounter = new PerformanceCounter("Process", "Handle Count", "_Total", System.Environment.MachineName);

        private PerformanceCounter threadCount = new PerformanceCounter("Process", "Thread Count", "_Total");

        private PerformanceCounter contentSwitches = new PerformanceCounter("System", "Context Switches/sec", null);

        private PerformanceCounter systemCalls = new PerformanceCounter("System", "System Calls/sec", null);

        private PerformanceCounterCategory performanceNetCounterCategory;

        private PerformanceCounter[] trafficSentCounters;

        private PerformanceCounter[] trafficReceivedCounters;

        private string[] interfaces = null;

        public void detailsMemory()
        {
            machineName = System.Environment.MachineName;
            float tmp = cpuProcessorTime.NextValue();

            CPUProcessorTime = (float)(Math.Round((double)tmp, 1));


            var pageFile = getPageFile();

           var ProcessorQueueLengh = getProcessorQueueLengh();
            var MemCommite = getMemCommited();


            var MemPoolPaged = getMemPoolPaged();
            var MemPoolNonPaged = getMemPoolNonPaged();
            var MemCachedBytes = getMemCachedBytes();
            var MemAvailable = getMemAvailable();

            
        }
        public void initNetCounters()

        {
            machineName = System.Environment.MachineName;


            // PerformanceCounter(CategoryName,CounterName,InstanceName)

            performanceNetCounterCategory = new PerformanceCounterCategory("Network Interface");

            interfaces = performanceNetCounterCategory.GetInstanceNames();

            int length = interfaces.Length;

            if (length > 0)

            {

                trafficSentCounters = new PerformanceCounter[length];

                trafficReceivedCounters = new PerformanceCounter[length];

            }

            for (int i = 0; i < length; i++)

            {

                // Initializes a new, read-only instance of the PerformanceCounter class.

                // 1st paramenter: "categoryName"-The name of the performance counter category (performance object) with which this performance counter is associated.

                // 2nd paramenter: "CounterName" -The name of the performance counter.

                // 3rd paramenter: "instanceName" -The name of the performance counter category instance, or an empty string (""), if the category contains a single instance.

                trafficReceivedCounters[i] = new PerformanceCounter("Network Interface", "Bytes Sent/sec", interfaces[i]);

                trafficSentCounters[i] = new PerformanceCounter("Network Interface", "Bytes Sent/sec", interfaces[i]);

            }

            // List of all names of the network interfaces

            for (int i = 0; i < length; i++)

            {

                Console.WriteLine("Name netInterface: {0}", performanceNetCounterCategory.GetInstanceNames()[i]);

            }


            getProcessorCpuTime();
            //getCpuPrivilegedTime();
            //getCpuinterruptTime();
            getcpuDPCTime();
            getPageFile();
            getProcessorQueueLengh();
            getMemCommited();
            getMemCommitLimit();
            getMemCommitedPerc();
            getMemPoolPaged();
            getMemPoolNonPaged();
            getMemCachedBytes();
            getMemAvailable();
            //getDiskQueueLengh();
            //getDiskRead();
            //getDiskWrite();
            //getDiskAverageTimeRead();
            //getDiskAverageTimeWrite();
            //getDiskTime();
            getHandleCountCounter();
            getThreadCount();
            getContentSwitches();
            getsystemCalls();
            getCurretTrafficSent();
            getCurretTrafficReceived();
         //   Console.ReadLine();

        }

        /// <summary>
        /// 
        /// </summary>
        public void getProcessorCpuTime()

        {

            float tmp = cpuProcessorTime.NextValue();

            CPUProcessorTime = (float)(Math.Round((double)tmp, 1));
            Console.WriteLine("getProcessorCpuTime: {0}", CPUProcessorTime);
            var news = string.Format("Processor\", \"% Processor Time\", \"_Total: {0}", CPUProcessorTime);
            CommonFunctions.LogWriteLines(news);
            // Environment.ProcessorCount: return the total number of cores

        }
        /// <summary>
        /// 
        /// </summary>
        public void getCpuPrivilegedTime()

        {

            float tmp = cpuPrivilegedTime.NextValue();

            CPUPrivilegedTime = (float)(Math.Round((double)tmp, 1));
            var news = string.Format("\"Processor\", \"% Privileged Time\", \"_Total\"", CPUPrivilegedTime.ToString());
            CommonFunctions.LogWriteLines(news);

        }
        /// <summary>
        /// 
        /// </summary>
        public void getCpuinterruptTime()

        {

            float tmp = cpuInterruptTime.NextValue();

            CPUInterruptTime = (float)(Math.Round((double)tmp, 1));
            var news = string.Format("Processor\", \"% Interrupt Time\", \"_Total: {0}", CPUInterruptTime.ToString());
            CommonFunctions.LogWriteLines(news);

        }
        /// <summary>
        /// 
        /// </summary>
        public void getcpuDPCTime()

        {

            float tmp = cpuDPCTime.NextValue();

            CPUDPCTime = (float)(Math.Round((double)tmp, 1));
            var news = string.Format("Processor\", \"% DPC Time\", \"_Total: {0}", CPUDPCTime.ToString());
            CommonFunctions.LogWriteLines(news);

        }

        /// <summary>
        /// 
        /// </summary>
        public float getPageFile()

        {

            PageFile = pageFile.NextValue();
            var news = string.Format("Paging File\", \"% Usage\", \"_Total: {0}", PageFile.ToString());
            CommonFunctions.LogWriteLines(news);
            return PageFile;

        }
        /// <summary>
        /// 
        /// </summary>
        public float getProcessorQueueLengh()

        {

            ProcessorQueueLengh = processorQueueLengh.NextValue();
            var news = string.Format("System\", \"Processor Queue Length: {0}", ProcessorQueueLengh.ToString());
            CommonFunctions.LogWriteLines(news);
            return ProcessorQueueLengh;

        }

        /// <summary>
        /// /
        /// </summary>
        public float getMemAvailable()

        {

            MEMAvailable = memAvailable.NextValue();
            Console.WriteLine("getMemAvailable: {0}", MEMAvailable);
            var news = string.Format("getMemAvailable: {0}", MEMAvailable.ToString());
            CommonFunctions.LogWriteLines(news);
            return MEMAvailable;

        }
        /// <summary>
        /// 
        /// </summary>
        public float getMemCommited()

        {

            MEMCommited = memCommited.NextValue() / (1024 * 1024);
            var news = string.Format("Memory\", \"Committed Bytes: {0}", MEMCommited);
            CommonFunctions.LogWriteLines(news);
            return MEMCommited;
        }

        /// <summary>
        /// 
        /// </summary>
        public void getMemCommitLimit()

        {

            MEMCommitLimit = memCommitLimit.NextValue() / (1024 * 1024);
            var news = string.Format("Memory\", \"Commit Limit: {0}", MEMCommitLimit);
            CommonFunctions.LogWriteLines(news);

        }

        /// <summary>
        /// 
        /// </summary>
        public void getMemCommitedPerc()

        {float tmp = memCommitedPerc.NextValue();

            // return the value of Memory Commit Limit

            MEMCommitedPerc = (float)(Math.Round((double)tmp, 1));

            
            var news = string.Format("Memory\", \"% Committed Bytes In Use: {0}", MEMCommitedPerc);
            CommonFunctions.LogWriteLines(news);

        }
        /// <summary>
        /// 
        /// </summary>
        public float getMemPoolPaged()

        {

            float tmp = memPollPaged.NextValue() / (1024 * 1024);

            MEMPoolPaged = (float)(Math.Round((double)tmp, 1));
            var news = string.Format("Memory\", \"Pool Paged Bytes: {0}", MEMPoolPaged);
            CommonFunctions.LogWriteLines(news);
            return MEMPoolPaged;
        }
        /// <summary>
        /// 
        /// </summary>
        public float getMemPoolNonPaged()

        {

            float tmp = memPollNonPaged.NextValue() / (1024 * 1024);

            MEMPoolNonPaged = (float)(Math.Round((double)tmp, 1));
            var news = string.Format("\"Memory\", \"Pool Nonpaged Bytes: {0}", MEMPoolNonPaged);
            
            CommonFunctions.LogWriteLines(news);

            return MEMPoolNonPaged;

        }

        /// <summary>
        /// 
        /// </summary>
        public float getMemCachedBytes()

        {

            // return the value of Memory Cached in MBytes

            MEMCached = memCached.NextValue() / (1024 * 1024);
            var news = string.Format("Memory\", \"Cache Bytes: {0}", MEMCached);
            CommonFunctions.LogWriteLines(news);
            return MEMCached;

        }
        /// <summary>
        /// 
        /// </summary>
        public void getDiskQueueLengh()

        {

            DISCQueueLengh = diskQueueLengh.NextValue();
            var news = string.Format("PhysicalDisk\", \"Avg. Disk Queue Length\", \"_Total: {0}", DISCQueueLengh);
            CommonFunctions.LogWriteLines(news);

        }
        /// <summary>
        /// 
        /// </summary>
        public void getDiskRead()

        {

            float tmp = diskRead.NextValue() / 1024;

            DISKRead = (float)(Math.Round((double)tmp, 1));
            var news = string.Format("PhysicalDisk\", \"Disk Read Bytes/sec\", \"_Total\": {0}", DISKRead);
            CommonFunctions.LogWriteLines(news);

        }
        /// <summary>
        /// 
        /// </summary>
        public void getDiskWrite()

        {

            float tmp = diskWrite.NextValue() / 1024;

            DISKWrite = (float)(Math.Round((double)tmp, 1)); // round 1 digit decimal
            var news = string.Format("PhysicalDisk\", \"Disk Write Bytes/sec\", \"_Total: {0}", DISKWrite);
            CommonFunctions.LogWriteLines(news);

        }
        /// <summary>
        /// 
        /// </summary>
        public void getDiskAverageTimeRead()

        {

            float tmp = diskAverageTimeRead.NextValue() * 1000;

            DISKAverageTimeRead = (float)(Math.Round((double)tmp, 1)); // round 1 digit decimal
            var news = string.Format("PhysicalDisk\", \"Avg. Disk sec/Read\", \"_Total: {0}", DISKAverageTimeRead);
            CommonFunctions.LogWriteLines(news);

        }

        public void getDiskAverageTimeWrite()

        {

            float tmp = diskAverageTimeWrite.NextValue() * 1000;

            DISKAverageTimeWrite = (float)(Math.Round((double)tmp, 1)); // round 1 digit decimal
            var news = string.Format("PhysicalDisk\", \"Avg. Disk sec/Write\", \"_Total: {0}", DISKAverageTimeWrite);
            CommonFunctions.LogWriteLines(news);

        }
        /// <summary>
        /// 
        /// </summary>
        public void getDiskTime()

        {

            float tmp = diskTime.NextValue();

            DISKTime = (float)(Math.Round((double)tmp, 1));
            var news = string.Format("PhysicalDisk\", \"% Disk Time\", \"_Total: {0}", DISKTime);
            CommonFunctions.LogWriteLines(news);

        }



        public void getHandleCountCounter()

        {

            HANDLECountCounter = handleCountCounter.NextValue();
            var news = string.Format("HandleCountCounter: {0}", HANDLECountCounter);
            CommonFunctions.LogWriteLines(news);

        }

        public void getThreadCount()

        {

            THREADCount = threadCount.NextValue();
            var news = string.Format("Process\", \"Handle Count\", \"_Total: {0}", THREADCount);
            CommonFunctions.LogWriteLines(news);

        }

        public void getContentSwitches()

        {

            CONTENTSwitches = (int)Math.Ceiling(contentSwitches.NextValue());
            var news = string.Format("System\", \"Context Switches/sec: {0}", CONTENTSwitches);
            CommonFunctions.LogWriteLines(news);

        }

        public void getsystemCalls()

        {

            SYSTEMCalls = (int)Math.Ceiling(systemCalls.NextValue());
            var news = string.Format("System\", \"System Calls/sec: {0}", SYSTEMCalls);
            CommonFunctions.LogWriteLines(news);

        }

        public void getCurretTrafficSent()

        {

            int length = interfaces.Length;

            float sendSum = 0.0F;

            for (int i = 0; i < length; i++)

            {

                sendSum += trafficSentCounters[i].NextValue();

            }

            float tmp = 8 * (sendSum / 1024);

            NetTrafficSend = (float)(Math.Round((double)tmp, 1));

            var news = string.Format("Net Traffic Send Curret Traffic Sent: {0}", NetTrafficSend);
            CommonFunctions.LogWriteLines(news);

        }

        public void getCurretTrafficReceived()

        {

            int length = interfaces.Length;

            float receiveSum = 0.0F;

            for (int i = 0; i < length; i++)

            {

                receiveSum += trafficReceivedCounters[i].NextValue();

            }

            float tmp = 8 * (receiveSum / 1024);

            NetTrafficReceive = (float)(Math.Round((double)tmp, 1));

            var news = string.Format("Net Traffic Receive Curret Traffic Received: {0}", NetTrafficReceive);
            CommonFunctions.LogWriteLines(news);

        }

        public void getSampleTime()

        {

            SamplingTime = DateTime.Now;

        }

    }
}
