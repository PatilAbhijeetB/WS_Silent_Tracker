using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow
{
    public static class UrlCaptureHandler
    {
        public static string GetRunningBrowserUrl(string appName, string ProcessId)
        {
            try
            {
                if (appName == "msedge")
                {
                    foreach (Process process in Process.GetProcessesByName("msedge"))
                    {
                        string url = GeMsEdgeUrl(process);
                        if (url == null)
                            continue;


                        return url;
                    }
                }

                if (appName == "chrome")
                {

                    foreach (Process process in Process.GetProcessesByName("chrome"))
                    {
                        string url = GetChromeUrl3(process);
                        if (url == null)
                            continue;

                        return url;
                    }
                }


                if (appName == "opera")
                {

                    foreach (Process process in Process.GetProcessesByName("opera"))
                    {
                        string url = GetOperaURLs(process);
                        if (url == null)
                            continue;
                        //   MessageBox.Show(url);
                        return url;
                    }

                }

                if (appName == "firefox")
                {
                    string url = getFireFoxUrlOnEveryBrowser();
                    return url;

                }

                if (appName == "iexplore")
                {

                    foreach (Process process in Process.GetProcessesByName("iexplore"))
                    {
                        string url = GetInternetExplorerUrl(process);
                        if (url == null)
                            continue;

                        return url;
                    }
                }

                return null;

            }
            catch (Exception ex)
            {
                //var RetunExceptionValue = new MessageStatus
                //{
                //    Status = "500",
                //    Url = "",
                //    Massage = ex.Message

                //};
                //var jsons = new JavaScriptSerializer().Serialize(RetunExceptionValue);
                //Console.WriteLine(jsons);
                return null;
            }


        }

        public static string GetOperaURLs(Process process)
        {

            try
            {
               
                // the chrome process must have a window
                if (process == null)
                    throw new ArgumentNullException("process");

                if (process.MainWindowHandle == IntPtr.Zero)
                    return null;

                AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element == null)
                    return null;

                AutomationElement doc = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address field"));

                if (doc == null)
                    return null;

                AutomationPattern pattern = doc.GetSupportedPatterns().FirstOrDefault(wr => wr.ProgrammaticName == "ValuePatternIdentifiers.Pattern");
                if (pattern == null)
                    return null;

                ValuePattern val = (ValuePattern)doc.GetCurrentPattern(pattern);
                var activeUrl = val.Current.Value;

                return activeUrl;


            }
            catch (Exception ex)
            {

                return null;

            }

        }
        public static string GeMsEdgeUrl(Process process)
        {
            try
            {
               
                if (process == null)
                    throw new ArgumentNullException("process");

                if (process.MainWindowHandle == IntPtr.Zero)
                    return null;

                AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element == null)
                    return null;

                AutomationElement doc = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));
                // AutomationElement docd = element.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));

                if (doc == null)
                    return null;
                var activeUrl = ((ValuePattern)doc.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;

                return activeUrl;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public static string GetChromeUrl3(Process proc)
        {
            try
            {
                
                string procname = proc.ProcessName;
                // the chrome process must have a window    
                if (proc.MainWindowHandle == IntPtr.Zero)
                    return null;

                AutomationElement element = AutomationElement.FromHandle(proc.MainWindowHandle);
                if (element == null)
                    return null;

                AutomationElement doc = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

                if (doc == null)
                    return null;
                var activeUrl = (string)doc.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);

                return activeUrl;


            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public static string getFireFoxUrlOnEveryBrowser()
        {
            try
            {
                AutomationElement root = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass"));

                Condition toolBar = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar),
                new PropertyCondition(AutomationElement.NameProperty, "Browser tabs"));
                var tool = root.FindFirst(TreeScope.Children, toolBar);

                var tool2 = TreeWalker.ControlViewWalker.GetNextSibling(tool);

                var children = tool2.FindAll(TreeScope.Children, Condition.TrueCondition);

               

                foreach (AutomationElement item in children)
                {
                    foreach (AutomationElement i in item.FindAll(TreeScope.Children, Condition.TrueCondition))
                    {
                        foreach (AutomationElement ii in i.FindAll(TreeScope.Element, Condition.TrueCondition))
                        {
                            if (ii.Current.LocalizedControlType == "edit")
                            {
                                if (!ii.Current.BoundingRectangle.X.ToString().Contains("empty"))
                                {
                                    ValuePattern activeTab = ii.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                                    var activeUrl = activeTab.Current.Value;

                                    //RetunValue = new MessageStatus
                                    //{
                                    //    Status = "200",
                                    //    Url = activeUrl,
                                    //    Massage = "Success"

                                    //};
                                    //json = new JavaScriptSerializer().Serialize(RetunValue);
                                    // Console.WriteLine(json);
                                    return activeUrl;
                                }

                                // return null;
                            }
                            //  return null;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {

                return null;

            }


        }


        public static string GetInternetExplorerUrl(Process process)
        {
            try
            {
                
                if (process == null)
                    throw new ArgumentNullException("process");

                if (process.MainWindowHandle == IntPtr.Zero)
                    return null;

                AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element == null)
                    return null;

                AutomationElement rebar = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "ReBarWindow32"));
                if (rebar == null)
                    return null;

                AutomationElement edit = rebar.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                if (edit == null)
                    return null;

                var activeUrl = ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;

                return activeUrl;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
