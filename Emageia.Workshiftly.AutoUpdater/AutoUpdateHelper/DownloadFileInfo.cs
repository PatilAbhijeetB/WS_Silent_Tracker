/*****************************************************************

 * Author:   Sameera Niroshan 
 * Email:    sameera@emagia.com
 * Create Date:  18/02/2022 
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper
{
    public class DownloadFileInfo
    {
        #region Privete Fields

        //internal string fileName;
        //internal long fileSize;
        //internal string downloadUrl;
        //internal string md5;
        //internal Version version;
        //private Uri uri;
        //private string description;
        //private string launchArgs;

        #endregion

        #region Public Properties
       
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string DownloadUrl { get; set; }
        public string MD5 { get; set; }
        public Version Version { get; set; }
        public Uri Uri { get; set; }
        public string Description { get; set; }
        public string LaunchArgs { get; set; }

        //public string FileName { get { return fileName; }; set { fileName = value; } }
        //public long FileSize { get { return fileSize; } }
        //public string DownloadUrl { get { return downloadUrl; } }
        //public string MD5 { get { return md5; } }
        //public Version Version { get { return version; } }
        //internal Uri Uri { get { return uri; } }
        //internal string Description { get { return description; } }
        //internal string LaunchArgs { get { return launchArgs; } }
        #endregion
    }
}
