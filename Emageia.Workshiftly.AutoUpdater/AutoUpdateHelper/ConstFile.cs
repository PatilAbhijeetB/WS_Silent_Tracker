using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper
{
    public class ConstFile
    {
        public const string TEMPFOLDERNAME = "TempFolder";
        public const string OLDFOLDERNAME = "OldFolder";
       // public const string OLDFOLDERNAME = "OldFolder";
        public const string CONFIGFILEKEY = "config_";
        public const string FILENAME = "AutoUpdater.config";
        public const string ROOLBACKFILE = "Emageia.Workshiftly.MainApplication.exe";
        public const string MESSAGETITLE = "AutoUpdate Program";
        public const string CANCELORNOT = "Workshiftly Update is in progress. Do you really want to cancel?";
        public const string APPLYTHEUPDATE = "Program need to restart to apply the update,Please click OK to restart the program!";
        public const string NOTNETWORK = "Workshiftly.exe update is unsuccessful. Workshiftly.exe will now restart. Please try to update again.";

        public static string SystemBinUrl { get; internal set; }
    }
}
