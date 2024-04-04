using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow
{
    public static class UrlCaptureHnadlerTesting
    {
        public static string GetRunningBrowserUrl(string appName, IntPtr ProcessId)
        {
            try
            {
                //int pid = int.Parse(ProcessId);
                IntPtr pid = ProcessId;

                if (appName == "EDGE")
                {
                    string url = GeMsEdgeUrl(pid);
                    return url;
                }

                if (appName == "CHROME")
                {
                    string url = GetChromeUrl3(pid);
                    return url;
                }

                if (appName == "OPERA")
                {
                    string url = GetOperaURLs(pid);
                    return url;
                }

                if (appName == "FIREFOX")
                {

                    string url = getFireFoxUrl(pid);
                    return url;
                }

                if (appName == "IEXPLORER")
                {
                    string url = GetInternetExplorerUrl(pid);
                    return url;
                }

                return null;
            }
            catch (Exception ex)
            {

                return null;
            }


        }
        public static string getFireFoxUrl(IntPtr MainWindowHandle)
        {
            try
            {
                if ((IntPtr)MainWindowHandle == IntPtr.Zero)
                    return null;

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
                            if (ii.Current.LocalizedControlType == "edit" && !ii.Current.BoundingRectangle.X.ToString().Contains("empty"))
                            {
                                ValuePattern activeTab = ii.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                                var activeUrl = activeTab.Current.Value;

                                return activeUrl;
                            }

                            continue;
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
        public static string GetOperaURLs(IntPtr MainWindowHandle)
        {
            try
            {

                // the chrome process must have a window
                if ((IntPtr)MainWindowHandle == IntPtr.Zero)
                    return null;

                AutomationElement element = AutomationElement.FromHandle((IntPtr)MainWindowHandle);
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
        public static string GeMsEdgeUrl(IntPtr MainWindowHandle)
        {
            try
            {

                if ((IntPtr)MainWindowHandle == IntPtr.Zero)
                    return null;

                AutomationElement element = AutomationElement.FromHandle((IntPtr)MainWindowHandle);
                if (element == null)
                    return null;

                AutomationElement doc = element.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));

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

        public static string GetChromeUrl3(IntPtr MainWindowHandle)
        {
            try
            {

                if ((IntPtr)MainWindowHandle == IntPtr.Zero)
                    return null;

                Console.WriteLine("before", (IntPtr)MainWindowHandle);
                AutomationElement element = AutomationElement.FromHandle((IntPtr)MainWindowHandle);
                if (element == null)
                    return null;


                Console.WriteLine("before doc", (IntPtr)MainWindowHandle);
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
                            if (ii.Current.LocalizedControlType == "edit" && !ii.Current.BoundingRectangle.X.ToString().Contains("empty"))
                            {
                                ValuePattern activeTab = ii.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                                var activeUrl = activeTab.Current.Value;

                                return activeUrl;
                            }

                            return null;
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


        public static string GetInternetExplorerUrl(IntPtr MainWindowHandle)
        {
            try
            {

                if ((IntPtr)MainWindowHandle == IntPtr.Zero)
                    return null;

                AutomationElement element = AutomationElement.FromHandle((IntPtr)MainWindowHandle);
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
