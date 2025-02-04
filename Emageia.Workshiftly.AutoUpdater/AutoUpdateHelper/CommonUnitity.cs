﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper
{
    public class CommonUnitity
    {
        public static string SystemBinUrl = AppDomain.CurrentDomain.BaseDirectory;

        public static void RestartApplication()
        {
            Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
        }

        public static string GetFolderUrl(DownloadFileInfo file)
        {
            string folderPathUrl = string.Empty;
            int folderPathPoint = file.DownloadUrl.IndexOf("/", 15) + 1;
            string filepathstring = file.DownloadUrl.Substring(folderPathPoint);
            int folderPathPoint1 = filepathstring.IndexOf("/");
            string filepathstring1 = filepathstring.Substring(folderPathPoint1 + 1);
            if (filepathstring1.IndexOf("/") != -1)
            {
                string[] ExeGroup = filepathstring1.Split('/');
                for (int i = 0; i < ExeGroup.Length - 1; i++)
                {
                    folderPathUrl += "\\" + ExeGroup[i];
                }
                if (!Directory.Exists(SystemBinUrl + ConstFile.TEMPFOLDERNAME + folderPathUrl))
                {
                    Directory.CreateDirectory(SystemBinUrl + ConstFile.TEMPFOLDERNAME + folderPathUrl);
                }
            }
            return folderPathUrl;
        }

        public static string GetFolderUrls(DownloadFileInfo file)
        {
            string[] paths = { $@"{SystemBinUrl}," };
            string folderPathUrl = Path.Combine(SystemBinUrl, ConstFile.TEMPFOLDERNAME, file.FileName);


            if (!Directory.Exists(SystemBinUrl + ConstFile.OLDFOLDERNAME))
            {
                Directory.CreateDirectory(SystemBinUrl + ConstFile.TEMPFOLDERNAME);
            }


            return folderPathUrl;
        }
    }
}
