using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture
{
    public class ScreenCaptureActiveWindow
    {

        public ScreenCaptureActiveWindow()
        {

        }

        public void captureScreen()
        {
            // int i = 1;
            string root = @"C:\TimeTrack";
            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {

                String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Graphics memoryGraphics = Graphics.FromImage(screenshot);
                memoryGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

                MemoryStream ms = new MemoryStream();
                screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                var details = ms.ToArray();


                screenshot.Save(root + screen.DeviceName + filename);
            }

            //var size = GetHostingScreenSize(screen.DeviceName);


        }
    }
}
