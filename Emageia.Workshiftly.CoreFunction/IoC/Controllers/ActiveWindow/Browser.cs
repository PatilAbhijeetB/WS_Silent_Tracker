using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow
{
    public class Browser
    {

        public Browser(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static Browser OPERA { get { return new Browser("opera"); } }
        public static Browser CHROME { get { return new Browser("chrome"); } }
        public static Browser FIREFOX { get { return new Browser("firefox"); } }
        public static Browser EDGE { get { return new Browser("msedge"); } }
        public static Browser IEXPLORER { get { return new Browser("iexplorer"); } }

        public override string ToString()
        {
            return base.ToString();
        }
        public static bool isWebBrowser(string appName)
        {
            switch (appName)
            {
                case "opera":
                    return true;
                    break;
                case "chrome":
                    return true;
                    break;
                case "firefox":
                    return true;
                    break;
                case "msedge":
                    return true;
                    break;
                case "iexplorer":
                    return true;
                    break;
                default:
                    return false;

            }

        }


        public static string WebBrowsersName(string appName)
        {
            switch (appName)
            {
                case "opera":
                    return "OPERA";
                    break;
                case "chrome":
                    return "CHROME";
                    break;
                case "firefox":
                    return "FIREFOX";
                    break;
                case "msedge":
                    return "EDGE";
                    break;
                case "iexplorer":
                    return "IEXPLORER";
                    break;
                default:
                    return "";

            }

        }
    }
}
