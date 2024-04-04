using System;

namespace Emageia.Workshiftly.CoreFunction.IoC.Update
{
    internal class DownloadFileInfo
    {
        internal string fileName;
        internal long fileSize;
        internal string downloadUrl;
        internal string md5;
        internal Version version;
        private Uri uri;
        private string description;
        private string launchArgs;




        public string FileName { get { return fileName; } set { fileName = value; } }
        public long FileSize { get { return fileSize; } }
        public string DownloadUrl { get { return downloadUrl; } }
        public string MD5 { get { return md5; } }
        public Version Version { get { return version; } }
        internal Uri Uri { get { return uri; } }
        internal string Description { get { return description; } }
        internal string LaunchArgs { get { return launchArgs; } }
    }
}