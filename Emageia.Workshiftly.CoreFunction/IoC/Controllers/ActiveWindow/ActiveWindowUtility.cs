using Emageia.Workshiftly.CoreFunction.IoC.HalAccess;
using Emageia.Workshiftly.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Emageia.Workshiftly.Entity;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using System.Text;
using Emageia.Workshiftly.CoreFunction.IoC.Service.SocketServer;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow
{
    public class ActiveWindowUtility
    {
        public static string appName, prevvalue, title, prevTitle, prevAppName, NewAppName, newTitle;
        public static string CurrentUrl, PreveUrl;
        public static Int32 ProcessId, PrevProcessId, StartedTimeStamp, EndTimeStamp, focusDuration;
        public static bool IsBrowser, PrevIsBrowser, isNewAppl;
        public static ActivedWindow CURRENTACTIVIITYWINDOW = new ActivedWindow();
        public static ActivedWindow LASTACTIVIITYWINDOW = new ActivedWindow();
        public ActiveWindowUtility()
        {

        }


        public void activeWindow()
        {
            try
            {

                var activeUrl = string.Empty;

                #region Get Current active window and details

                DateTime stdate = DateTime.Now;
                // get active window handler
                IntPtr hwnd = APIFuncs.getforegroundWindow();

                // get process Id active window
                Int32 pid = APIFuncs.GetWindowProcessID(hwnd);
                Process p = Process.GetProcessById(pid);

                var appNames = p.ProcessName;
                var appltitle = APIFuncs.ActiveApplTitle().Trim().Replace("\0", "");

                //Int32 currentTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

                #endregion


                #region Assign value to variables

                #endregion
                var currentUrlchek = string.Empty;
                if (appNames != null && Browser.isWebBrowser(appNames))
                {
                    stdate = DateTime.Now;
                    var date = DateTime.Now;
                    CurrentUrl = activeUrl = UrlCaptureHnadlerTesting.GetRunningBrowserUrl(Browser.WebBrowsersName(appNames), p.MainWindowHandle);

                    var dates = DateTime.Now;


                }
                DateTime endTime = DateTime.Now;
                // check the active window is browser or not
                _ = Browser.isWebBrowser(appNames) ? IsBrowser = true : IsBrowser = false;


                #region check Conditionals
                // Check the active window is change
                if (appName != appNames || title != appltitle)
                {
                    if (isTyping(appltitle, prevTitle, CalculateSimilarity(CurrentUrl, PreveUrl), CurrentUrl, IsBrowser))
                    {
                        appName = appNames;
                        title = appltitle;
                        newTitle = appltitle;
                        NewAppName = appName;
                        ProcessId = pid;
                        var ne = ProcessId.ToString();
                        isNewAppl = true;
                    }
                    else
                    {
                        isNewAppl = false;
                    }

                }
                else
                {
                    isNewAppl = false;

                }


                #endregion


                #region Store data

                if (prevAppName != appName || prevTitle != title)
                {

                    if (isTyping(appltitle, prevTitle, CalculateSimilarity(CurrentUrl, PreveUrl), CurrentUrl, IsBrowser))
                    {
                        EndTimeStamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

                        if (!String.IsNullOrEmpty(prevAppName) && !String.IsNullOrEmpty(prevTitle))
                        {
                            try
                            {
                                List<ActivietyWindows> activietyWindows = new List<ActivietyWindows>();
                                var applicationActiveModel = new ActivietyWindows();

                                applicationActiveModel.appName = prevAppName;
                                applicationActiveModel.title = Title(PrevIsBrowser, prevTitle, PreveUrl, prevAppName);
                                applicationActiveModel.processId = PrevProcessId.ToString();
                                applicationActiveModel.startedTimestamp = StartedTimeStamp;
                                applicationActiveModel.endTimestamp = EndTimeStamp;
                                applicationActiveModel.focusDuration = EndTimeStamp - StartedTimeStamp;
                                applicationActiveModel.companyId = CommonUtility.UserSessions.companyId;
                                applicationActiveModel.userId = CommonUtility.UserSessions.id;
                                applicationActiveModel.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                                applicationActiveModel.operatingSystem = "Windows";
                                applicationActiveModel.isSynced = false;
                                activietyWindows.Add(applicationActiveModel);
                              // bool success =
                               // var nio = HttpRequestSocketCaller.syncShocketActivityWindows(activietyWindows);
                                HttpRequestSocketCaller.syncShocketActivityWindows(activietyWindows);

                                //using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                                //{
                                //    //var applicationActiveModel = new ActivietyWindows();

                                //    //applicationActiveModel.appName = prevAppName;
                                //    //applicationActiveModel.title = Title(PrevIsBrowser, prevTitle, PreveUrl, prevAppName);
                                //    //applicationActiveModel.processId = PrevProcessId.ToString();
                                //    //applicationActiveModel.startedTimestamp = StartedTimeStamp;
                                //    //applicationActiveModel.endTimestamp = EndTimeStamp;
                                //    //applicationActiveModel.focusDuration = EndTimeStamp - StartedTimeStamp;
                                //    //applicationActiveModel.companyId = CommonUtility.UserSessions.companyId;
                                //    //applicationActiveModel.userId = CommonUtility.UserSessions.id;
                                //    //applicationActiveModel.deviceId = CommonUtility.DeviceSettings.id;
                                //    //applicationActiveModel.operatingSystem = "Windows";
                                //    //applicationActiveModel.isSynced = false;


                                //    _clientDbContext.ActivietyWindows.Add(applicationActiveModel);
                                //    _clientDbContext.SaveChanges();


                                //}

                                CommonUtility.LogWriteLines("Success", "ActiveUtility activeWindow", "CurrentUrl === " + (IsBrowser ? PreveUrl : title), "**Second************" + (endTime - stdate).Seconds.ToString());


                            }
                            catch (Exception ex)
                            {
                                CommonUtility.LogWriteLines("Error", "ActiveUtility activeWindow", "ScreenShot Run Utility Runnable => ActiveWindo Database Writing error", ex.ToString());
                            }
                        }
                        else
                        {
                            var check = prevAppName != appName || prevTitle != title;
                            //TODO = New value check
                        }

                        prevAppName = appName;
                        prevTitle = title;
                        PrevProcessId = pid;
                        PreveUrl = CurrentUrl;
                        PrevIsBrowser = IsBrowser;
                    }


                }
                else
                {
                    var check = prevAppName != appName || prevTitle != title;
                }

                if (isNewAppl)
                {
                    StartedTimeStamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                }

                #endregion

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "ActiveWindowUtility activeWindow", ex.InnerException.Message.ToString(), ex.ToString());

            }

        }


        public static void saveActiveWindowStopIdle(Int32 endTimeStamp)
        {

            //EndTimeStamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            EndTimeStamp = endTimeStamp > 0  ? endTimeStamp :Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

            if (!String.IsNullOrEmpty(prevAppName) && !String.IsNullOrEmpty(prevTitle))
            {
                try
                {
                    var activeWindowUtil = new ActiveWindowUtility();
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        var applicationActiveModel = new ActivietyWindows();

                        applicationActiveModel.appName = prevAppName;
                        applicationActiveModel.title = activeWindowUtil.Title(PrevIsBrowser, prevTitle, PreveUrl, prevAppName);
                        applicationActiveModel.processId = PrevProcessId.ToString();
                        applicationActiveModel.startedTimestamp = StartedTimeStamp;
                        applicationActiveModel.endTimestamp = EndTimeStamp;
                        applicationActiveModel.focusDuration = EndTimeStamp - StartedTimeStamp;
                        applicationActiveModel.companyId = CommonUtility.UserSessions.companyId;
                        applicationActiveModel.userId = CommonUtility.UserSessions.id;
                        applicationActiveModel.deviceId = CommonUtility.LoggedDeviceSettings.deviceID;
                        applicationActiveModel.operatingSystem = "Windows";
                        applicationActiveModel.isSynced = false;


                        _clientDbContext.ActivietyWindows.Add(applicationActiveModel);
                        _clientDbContext.SaveChanges();


                    }

                    prevAppName = null;
                    prevTitle = null;
                    PrevProcessId = 0;
                    PreveUrl = null;
                    PrevIsBrowser = false;
                }
                catch (Exception ex)
                {
                    CommonUtility.LogWriteLines("Error", "ActiveUtility saveActiveWindowStopIdle", "ScreenShot Run Utility Runnable => ActiveWindo Database Writing error", ex.ToString());
                }
            }

        }

        /// <summary>
        /// 1. title same     ->  url not same
        /// 2. title not same ->  url same
        /// 3. title not same ->  
        /// </summary>
        /// <param name="title"></param>
        /// <param name="preTitle"></param>
        /// <param name="precentage"></param>
        /// <param name="CurrentUrl"></param>
        /// <param name="isBrowser"></param>
        /// <returns></returns>
        private bool isTyping(string title, string preTitle, double precentage, string CurrentUrl, bool isBrowser)
        {
            if (isBrowser)
            {
                if (title == preTitle && (precentage <= 0.8) && (precentage < 1))
                {
                    PreveUrl = CurrentUrl;
                    return false;
                }
                else if (title != preTitle && precentage == 1)
                {
                    PreveUrl = CurrentUrl;
                    preTitle = title;
                    return false;
                }
                else
                {
                    return true;
                }
                //else if(title != preTitle && precentage == 1)
                //{
                //    return false;
                //}
            }

            return true;
        }
        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary>
        int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }


        /// <summary>
        /// Calculate percentage similarity of two strings
        /// <param name="source">Source String to Compare with</param>
        /// <param name="target">Targeted String to Compare</param>
        /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            double similarity = (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
            return Math.Round(similarity, 1);
        }



        #region Acitive window title

        /// <summary>
        /// /
        /// </summary>
        /// <param name="prevIsBrowser">befor active window is browser or not</param>
        /// <param name="title"> active window titel</param>
        /// <param name="preveUrl">preve acitve window Url</param>
        /// <param name="appName">running application Name</param>
        /// <returns> Url or Application Title </returns>

        public string Title(bool prevIsBrowser, string title, string preveUrl, string appName)
        {
            if (prevIsBrowser)
            {
                var titels = title != null ? title : appName;
                var returnTitel = "";
                if (preveUrl == null || preveUrl == "")
                {
                    returnTitel = titels;
                }
                else
                {
                    returnTitel = Urlhost(preveUrl);

                }
                return (preveUrl == null || preveUrl == "") ? titels : returnTitel;
            }
            else
            {
                if (appName == "explorer")
                {
                    if (title == "unknown")
                    {
                        return appName;
                    }
                    else if (title == null)
                    {
                        return appName;
                    }
                    else
                    {
                        return title;
                    }
                }
                return title;
            }
        }


        /// <summary>
        /// due to windows some machine url hasn't www ,http or https .
        /// our fileration on back-end need
        /// </summary>
        /// <param name="preveUrl">pass the url</param>
        /// <returns></returns>
        private string Urlhost(string preveUrl)
        {
            var startsWithHttps = preveUrl.StartsWith("https://");
            var startsWithWWW = preveUrl.StartsWith("www.");
            var startsWithhttp = preveUrl.StartsWith("http://");

            if (startsWithHttps)
            {
                return AppendWWW(preveUrl, startsWithHttps, "https://");
            }
            else if (startsWithhttp)
            {
                return AppendWWW(preveUrl, startsWithHttps, "http://");
            }
            else if (startsWithWWW)
            {
                return preveUrl;
            }

            return AppendWWW(preveUrl, false, null); ;
        }

        private string AppendWWW(string preveUrl, bool IsWith, string Type)
        {
            var processUrl = string.Empty;
            var isHost = preveUrl.Contains("www.");
            if (isHost)
            {
                processUrl = preveUrl.Replace(Type, "");
            }
            else if (String.IsNullOrEmpty(Type) && !isHost)
            {
                var stringBuilder = new StringBuilder("www.");
                stringBuilder = stringBuilder.Append(preveUrl);
                processUrl = stringBuilder.ToString();
            }
            else
            {
                processUrl = preveUrl.Replace(Type, "www.");
            }

            return processUrl;
        }

        #endregion



        
    }
}

