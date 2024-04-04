using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow;
using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture;
using Emageia.Workshiftly.CoreFunction.IoC.Service.Http;
using Emageia.Workshiftly.CoreFunction.IoC.SocketIoClient;
using Emageia.Workshiftly.Entity;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Emageia.Workshiftly.CoreFunction.IoC.Utility
{
    public static class CommonUtility
    {
        public static Thread syncActiveWindow { get; set; } = new Thread(() => HttpRequestCaller.syncActivityWindows());
        public static Thread syncScreenShot { get; set; }
        public static bool HasLogging { get; set; } = false;
        public static string Status { get; set; } 
        public static DispatcherTimer ActiveWindoDispatcherTimer { get; set; }
        public static DispatcherTimer ScreenCaptureUtility { get; set; }
        public static DispatcherTimer IdledispatcherTimer { get; set; }
      //  DispatcherTimer dispatcherTimer = new DispatcherTimer();


        public static UserSessions UserSessions { get; set; } =new UserSessions();
        public static CompanySettings CompanySettings { get; set; } = new CompanySettings();
        public static Company Company { get; set; } = new Company();
        public static CompanyConfiguration CompanyConfigurations { get; set; } = new CompanyConfiguration();
        public static DeviceConfiguration DeviceSettings { get; set; } = new DeviceConfiguration();
        public static DeviceSetting LoggedDeviceSettings { get; set; } = new DeviceSetting();

        //public SocketIO client;

        public static SocketIOAdapter socketIOAdapter { get; set; } = null;


        public static ScreenshotUtility screenCaptureUtilityFunction { get; set; } = new ScreenshotUtility();
        public static ActiveWindowUtility ActiveWindowUtilityFunction { get; set; } = new ActiveWindowUtility();



        public static Int32 currentIdleTime { get; set; }
        public static uint TickCountIdleTime { get; set; }
        public static bool IsIdleDbWrite { get; set; } = false;
        public static bool IsIdleListener { get; set; } =false;

        public static string SystemBinUrl = "";  //AppDomain.CurrentDomain.BaseDirectory;

        public static string LogPath { get; set; } = "";
        public static string SubFolderPath { get; set; } = "";
        public static string ImagePath { get; set; } = "";

     //   public static string ServerPath { get; set; } = "https://qa.workshiftly.com/api";
      //  public static string ServerPath { get; set; } = "https://staging-api.workshiftly.com/api";
           public static string ServerPath { get; set; } = "https://portal.workshiftly.com/api";



        public static void LogWriteLine(string msg)
        {
            try
            {
                string fileName = DateTime.Now.ToString("ddMMMyyyy_hhmmss");

                string[] line = new string[] { DateTime.Now.ToString() + "\t" + msg };
                File.AppendAllLines(LogPath, line);
            }
            catch (Exception ex)
            {

            }
        }
        public static void LogWriteLine(string msg,string name)
        {
            try
            {
                
                string[] line = new string[] { msg };
                File.AppendAllLines(LogPath, line);
            }
            catch (Exception ex)
            {

            }
        }

        public static void LogWriteLines(string msg)
        {
            try
            {
                string fileName = DateTime.Now.ToString("ddMMMyyyy_hhmmss");
                
                string[] line = new string[] { DateTime.Now.ToString() + "\t" + msg };
                File.AppendAllLines(LogPath, line);
            }
            catch (Exception ex)
            {

            }
        }

        public static void LogWriteLines(string type, string fuction, string result, string errorDescription)
        {
            try
            {
             
                string Date = "\n \n"+ DateTime.Now.ToString();

                //string name = string.Format("{0,20}{1,8}{2,18}{3,30}{4,26}",
                //        "Date", "Type", "Fuction", "Result", "Error Description");
                string name2 = string.Format("{0,20}{1,20}{2,29}{3,70}{4,26}",
                        Date, type, fuction, result, errorDescription);

                string[] line = new string[] { name2 };

                File.AppendAllLines(LogPath, line);



                //Console.WriteLine("{0,26}{1,8}{2,26}",
                //        "Argument", "Digits", "Result");


            }
            catch (Exception ex)
            {

            }
        }


        public static Int32 StartOfDay()
        {
            DateTime endOfThisDay = DateTime.Now;
            var Timestamp = new DateTime(endOfThisDay.Year, endOfThisDay.Month, endOfThisDay.Day, 0, 0, 0, 0);

            return Int32.Parse(new DateTimeOffset(Timestamp).ToUnixTimeSeconds().ToString());
            // long isn = new DateTimeOffset(Timestamp).ToUnixTimeSeconds();

        }

        public static Int32 EndOfDay()
        {
            DateTime endOfThisDay = DateTime.Now;
            var Timestamp = new DateTime(endOfThisDay.Year, endOfThisDay.Month, endOfThisDay.Day, 23, 59, 59, 999);

            return Int32.Parse(new DateTimeOffset(Timestamp).ToUnixTimeSeconds().ToString());
            // long isn = new DateTimeOffset(Timestamp).ToUnixTimeSeconds();

        }
        //internal static DispatcherTimer activeWindow

        public static DateTime UnixTimeStampToDateTime(Int32 unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static Int32 UnixTimeStampToDateTimeStartOfDay(Int32 unixTimeStamp)
        {
            
            DateTime startOfThisDay = UnixTimeStampToDateTime(unixTimeStamp);
            var Timestamp = new DateTime(startOfThisDay.Year, startOfThisDay.Month, startOfThisDay.Day, 0, 0, 0, 0);

            return Int32.Parse(new DateTimeOffset(Timestamp).ToUnixTimeSeconds().ToString());
        }

        public static Int32 UnixTimeStampToDateTimeEndOfDay(Int32 unixTimeStamp)
        {
            
            DateTime endOfThisDay = UnixTimeStampToDateTime(unixTimeStamp);
            var Timestamp = new DateTime(endOfThisDay.Year, endOfThisDay.Month, endOfThisDay.Day, 23, 59, 59, 999);

            return Int32.Parse(new DateTimeOffset(Timestamp).ToUnixTimeSeconds().ToString());
        }

        public static Int32 CompanyConfigurShiftTime(Int32 shitfTime)
        {
            if(shitfTime > 0)
            {
                return shitfTime / 3600;
            }
            return 0;
        }

        public static Int32 CompanyConfigurShiftTimes(Int32 shitfTime)
        {
            double shitf = (double)shitfTime / 3600;


            var decimalpartLength = shitf.ToString().Split('.');
            double min = decimalpartLength.Length == 2 ? 60 * double.Parse(("." + decimalpartLength[1])) : 0;

            Int32 hr = Int32.Parse(decimalpartLength[0]);


            DateTime dateTime = DateTime.Now;
            var Timestamp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hr, (Int32)min, 0, 0);
            return Int32.Parse(new DateTimeOffset(Timestamp).ToUnixTimeSeconds().ToString());
        }
    }
}
