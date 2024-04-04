using Emageia.Workshiftly.AutoUpdater.AutoUpdateHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emageia.Workshiftly.AutoUpdater.WindowFoms
{
    public partial class SharpUpdateInfoForm : Form
    {
        public SharpUpdateInfoForm(ISharpUpdatable applicaionInfo, SharpUpdateXml updateInfo)
        {
            InitializeComponent();

            this.Text = applicaionInfo.ApplicationName + "- Update Info";
            this.lblVersions.Text = String.Format("Current Version: {0}\nUpdate Version: {1}", applicaionInfo.ApplicationAssembly.GetName().Version.ToString(),
                 updateInfo.Version.ToString());
            this.txtDescription.Text = updateInfo.Description;
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if(!(e.Control && e.KeyCode == Keys.C))
                e.SuppressKeyPress = true;
        }

        //private void txtDescription_
    }
}
