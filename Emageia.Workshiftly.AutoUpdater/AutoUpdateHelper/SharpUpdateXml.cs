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
using System.Net;
using System.Xml;
namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper
{
   
    public class SharpUpdateXml
    {
        private Version version;
        private Uri uri;
        private Uri zipFileDownloadUri;
        private string fileName;
        private string md5;
        private string DowloadZipmd5;
        private string description;
        private string launchArgs;
        public List<DownloadFileInfo> downloadFileListTemp;

        internal Version Version { get { return this.version; } }
        internal Uri Uri { get { return this.uri; } }
        internal Uri ZipFileDownloadUri { get { return this.zipFileDownloadUri; } }
        internal string FileName { get { return this.fileName; } }
        internal string MD5 { get { return this.md5; } }
        internal string DownloadZipMD5 { get { return this.DowloadZipmd5; } }
        internal string Description { get { return this.description; } }
        internal string LaunchArgs { get { return this.launchArgs; } }
        public List<DownloadFileInfo> DownloadFileInfoLists { get { return this.downloadFileListTemp; } }

        internal SharpUpdateXml(Version version, Uri uri, string fileName, string md5, string description, string launchArg)
        {
            this.version = version;
            this.uri = uri;
            this.fileName = fileName;
            this.md5 = md5;
            this.description = description;
            this.launchArgs = launchArg;

        }
        public SharpUpdateXml(Version version, List<DownloadFileInfo> downloadFileListTemp,string DowloadZipmd5,Uri uri)
        {
            this.version = version;
            this.downloadFileListTemp = downloadFileListTemp;
            this.DowloadZipmd5 = DowloadZipmd5;
            this.zipFileDownloadUri = uri;

        }

        internal bool IsNewerThan(Version version)
        {
            return this.version > version;
        }
        public static bool ExistsOnServer(Uri loacation)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(loacation.AbsoluteUri);
                HttpWebResponse resq = (HttpWebResponse)req.GetResponse();
                resq.Close();

                return resq.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal static SharpUpdateXml Parse(Uri location, string appID)
        {
            Version mainVersions = null; Uri mainUri;
            
            try
            {
                List<DownloadFileInfo> downloadFileListTempNew = new List<DownloadFileInfo>();
               

                XmlDocument Xdoc = new XmlDocument();
                Xdoc.Load(location.AbsoluteUri);
                //Xdoc.Load(@"C:\Users\Niroshan\Desktop\My\HelloWorld\src\com\emageia\AutoUpdate.xml");

                XmlNodeList nodeList = Xdoc.DocumentElement.SelectNodes("/AutoUpdate");
                XmlNodeList nodeListd = Xdoc.DocumentElement.SelectNodes("/AutoUpdate/Update");
                //loop through each node and save it value in NodeStr

                var ApplicationVersion = Xdoc.DocumentElement.SelectSingleNode("/AutoUpdate/ApplicationVersion");
                mainVersions = new Version(ApplicationVersion.InnerText.ToString());
               
                var DownloadUrl = Xdoc.DocumentElement.SelectSingleNode("/AutoUpdate/Url").InnerText.ToString();
                var DownloadZipFileMd5 = Xdoc.DocumentElement.SelectSingleNode("/AutoUpdate/ZipMd5").InnerText.ToString();

                foreach (XmlNode node in nodeListd)
                {
                    //var download = new DownloadFileInfo();

                    var Version = node.SelectSingleNode("Version").InnerText;
                    var loaclaversions = new Version(Version.ToString());

                    var localuri = new Uri(node.SelectSingleNode("Url").InnerText);


                    var localFileName = node.SelectSingleNode("FileName").InnerText;

                    var localMD5 = node.SelectSingleNode("Md5").InnerText;

                    var localDescription = node.SelectSingleNode("Description").InnerText;


                    var localLaunchArgs = node.SelectSingleNode("LaunchArgs").InnerText;

                    //downloadFileListTempNew.Add(download);
                    downloadFileListTempNew.Add(insert(loaclaversions,localuri,localFileName,localMD5,localDescription,localLaunchArgs));

                }

                
                return new SharpUpdateXml(mainVersions, downloadFileListTempNew,DownloadZipFileMd5,new Uri(DownloadUrl));

            }
            catch (Exception)
            {

                return null;
            }

        }


        public static DownloadFileInfo insert(Version version, Uri uri, string fileName, string md5, string description, string launchArgs)
        {
            return new DownloadFileInfo { Version = version, Uri = uri, FileName = fileName, MD5 = md5, Description = description, LaunchArgs = launchArgs };
        }
    }
}






//internal static SharpUpdateXml Parse(Uri location, string appID)
//{
//    Version version = null;
//    string url = "", fileName = "", md5 = "", description = "", launchArgs = "";

//    try
//    {
//        XmlDocument doc = new XmlDocument();
//        doc.Load(location.AbsoluteUri);
//        //    @"C:\TimeTracker\SaveScreenShots\
//        //  doc.Load(@"C:\Users\Niroshan\Desktop\My\WordDemo\Word\App_Data\SaveCreateDoc\update.xml");

//        version = new Version(2, 0, 0, 0);
//        url = "https://localhost:44300/Api/TemplateDocument/DownlodDoc?fileName=TestApp.exe";
//        fileName = "TestApp.exe";
//        md5 = "bd5d97be20b1b690900bdc203d56a2c3";
//        description = "Initial update";
//        launchArgs = "";


//        //foreach (XmlNode nodes in doc.DocumentElement.ChildNodes)
//        //{
//        //    string text = nodes.InnerText; //or loop through its children as well
//        //}
//        //XmlNode node = doc.DocumentElement.SelectSingleNode("//update[@appId='" + appID + "'}");
//        //if (node == null)
//        //    return null;

//        //version = Version.Parse(node["version"].InnerText);
//        //url = node["url"].InnerText;
//        //fileName = node["fileName"].InnerText;
//        //md5 = node["md5"].InnerText;
//        //description = node["description"].InnerText;
//        //launchArgs = node["launchArgs"].InnerText;
//        List<DownloadFileInfo> downloadFileListTempx = null;
//        //   return new SharpUpdateXml(version, new Uri(url), fileName, md5, description, launchArgs);
//        return new SharpUpdateXml(downloadFileListTempx);

//    }
//    catch (Exception)
//    {

//        return null;
//    }

//}







