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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper
{
    public class ISharpUpdatable
    {
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ApplicationId { get; set; }
        public Assembly ApplicationAssembly { get; set; }
        public Icon ApplicationIcon { get; set; }
        public Uri UpdateXmlLocation { get; set; }
        public Form Context { get; set; }
    }
}
