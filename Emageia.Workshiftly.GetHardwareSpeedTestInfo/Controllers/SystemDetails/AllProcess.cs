using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.SystemDetails
{
    public class AllProcess
    {
        /// <summary>
        /// Returns an Expando object with the description and username of a process from the process ID.
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public ExpandoObject GetProcessExtraInformation(int processId)
        {
            // Query the Win32_Process
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            // Create a dynamic object to store some properties on it
            dynamic response = new ExpandoObject();
            response.Description = "";
            response.Username = "Unknown";

            foreach (ManagementObject obj in processList)
            {
                // Retrieve username 
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return Username
                    response.Username = argList[0];

                    // You can return the domain too like (PCDesktop-123123\Username using instead
                    //response.Username = argList[1] + "\\" + argList[0];
                }

                // Retrieve process description if exists
                if (obj["ExecutablePath"] != null)
                {
                    try
                    {
                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(obj["ExecutablePath"].ToString());
                        response.Description = info.FileDescription;
                    }
                    catch { }
                }
            }

            return response;
        }


        /// <summary>
        /// Method that converts bytes to its human readable value
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string BytesToReadableValue(long number)
        {
            List<string> suffixes = new List<string> { " B", " KB", " MB", " GB", " TB", " PB" };

            for (int i = 0; i < suffixes.Count; i++)
            {
                long temp = number / (int)Math.Pow(1024, i + 1);

                if (temp == 0)
                {
                    return (number / (int)Math.Pow(1024, i)) + suffixes[i];
                }
            }

            return number.ToString();
        }


        /// <summary>
        /// This method renders all the processes of Windows on a ListView with some values and icons.
        /// </summary>
        public void renderProcessesOnListView()
        {
            // Create an array to store the processes
            Process[] processList = Process.GetProcesses();

            // Create an Imagelist that will store the icons of every process
            //  ImageList Imagelist = new ImageList();

            // Loop through the array of processes to show information of every process in your console

           // List<string> moniters = new List<string>(osDetailsCollection.Count);
            foreach (Process process in processList)
            {
                // Define the status from a boolean to a simple string
                string status = (process.Responding == true ? "Responding" : "Not responding");

                // Retrieve the object of extra information of the process (to retrieve Username and Description)
                dynamic extraProcessInfo = GetProcessExtraInformation(process.Id);

                // Create an array of string that will store the information to display in our 
                string[] row = {
                // 1 Process name
                process.ProcessName,
                // 2 Process ID
                process.Id.ToString(),
                // 3 Process status
                status,
                // 4 Username that started the process
                extraProcessInfo.Username,
                // 5 Memory usage
                BytesToReadableValue(process.PrivateMemorySize64),
                // 6 Description of the process
                extraProcessInfo.Description
                };



        //        Console.WriteLine(@"
        //{0} | ID: {1} | Status {2} | Memory (private working set in Bytes) {3}  | virtual memory {4} | physical memory {5} ",
        //     process.ProcessName, process.Id, status, extraProcessInfo.Username, BytesToReadableValue(process.PrivateMemorySize64), extraProcessInfo.Description);
            };


        }


    }

}
