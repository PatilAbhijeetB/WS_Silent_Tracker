using Emageia.Workshiftly.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture
{
    public class ImageProcessHandler
    {

        public static void capture()
        {
            string WorkingDirectory = @"C:\Projects\Tutorials\ImageResize";

            ////create a image object containing a verticel photograph
            //Image imgPhotoVert = Image.FromFile(WorkingDirectory + @"\DISPLAY1ScreenCapture-29092022-083156.jpg");
            ////Image imgPhotoHoriz = Image.FromFile(WorkingDirectory + @"\imageresize_horiz.jpg");
            //Image imgPhoto = null;

            //imgPhoto = ScaleByPercentS(imgPhotoVert, 50);
            //imgPhoto.Save(WorkingDirectory + @"\images\imageresize_0.jpg", ImageFormat.Jpeg);
            //imgPhoto.Dispose();
            //imgPhoto = ScaleByPercentS2(imgPhotoVert, 50);
            //imgPhoto.Save(WorkingDirectory + @"\images\imageresize_0-1.jpg", ImageFormat.Jpeg);
            //imgPhoto.Dispose();
            //imgPhoto = ScaleByPercent(imgPhotoVert, 50);
            //imgPhoto.Save(WorkingDirectory + @"\images\imageresize_1.jpg", ImageFormat.Jpeg);
            //imgPhoto.Dispose();

            //imgPhoto = ConstrainProportions(imgPhotoVert, 200, Dimensions.Width);
            //imgPhoto.Save(WorkingDirectory + @"\images\imageresize_2.jpg", ImageFormat.Jpeg);
            //imgPhoto.Dispose();

            //imgPhoto = FixedSize(imgPhotoVert, 200, 200);
            //imgPhoto.Save(WorkingDirectory + @"\images\imageresize_3.jpg", ImageFormat.Jpeg);
            //imgPhoto.Dispose();

            //imgPhoto = Crop(imgPhotoVert, 200, 200, AnchorPosition.Center);
            //imgPhoto.Save(WorkingDirectory + @"\images\imageresize_4.jpg", ImageFormat.Jpeg);
            //imgPhoto.Dispose();

            //imgPhoto = Crop(imgPhotoHoriz, 200, 200, AnchorPosition.Center);
            //imgPhoto.Save(WorkingDirectory + @"\images\imageresize_5.jpg", ImageFormat.Jpeg);
            //imgPhoto.Dispose();
        }

        private static string takeCurrentScreenshot()
        {
            try
            {
                var width = 0;
                var height = 0;
                //var periodTimeSpan = TimeSpan.FromSeconds(1);
                int i = 1;
                string root = @"C:\TimeTrack";

                //   root = CommonUtility.ImagePath;
                var lstbitmap = new List<Bitmap>();

                foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    Bitmap image = MultiScreen2(screen, i, root);
                    lstbitmap.Add(image);


                    i++;
                    continue;

                }

                foreach (var image in lstbitmap)
                {
                    width += image.Width;
                    height = image.Height > height
                        ? image.Height
                        : height;
                }
                var bitmap2 = new Bitmap(width, height);
                var g = Graphics.FromImage(bitmap2);
                var localWidth = 0;

                foreach (var image in lstbitmap)
                {
                    g.DrawImage(image, localWidth, 0);
                    localWidth += image.Width;
                }

                var ms = new MemoryStream();
                String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                bitmap2.Save(ms, ImageFormat.Png);
                var result = ms.ToArray();
                bitmap2.Save(filename);

                return "";
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        private static Bitmap MultiScreen2(Screen screen, int i, string root)
        {
            try
            {

                //  CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs MultiScreen", "ScreenShot >>>>>  take Current Screenshot", "*******");

                String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Graphics memoryGraphics = Graphics.FromImage(screenshot);
                memoryGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

                return screenshot;


            }
            catch (Exception ex)
            {
                //     CommonUtility.LogWriteLines("Error", "ScreenshotUtility.cs MultiScreen", ex.Message.ToString(), ex.InnerException.ToString());

                return null;
            }
        }
        private static string MultiScreen(Screen screen, int i, string root)
        {
            try
            {

                //  CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs MultiScreen", "ScreenShot >>>>>  take Current Screenshot", "*******");

                String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Graphics memoryGraphics = Graphics.FromImage(screenshot);
                memoryGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

                MemoryStream ms = new MemoryStream();
                screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                var details = ms.ToArray();


                screenshot.Save(filename);
                //     CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs MultiScreen", "ScreenShot >>>>>  take Current Screenshot", "******");

                return Convert.ToBase64String(details);

            }
            catch (Exception ex)
            {
                //     CommonUtility.LogWriteLines("Error", "ScreenshotUtility.cs MultiScreen", ex.Message.ToString(), ex.InnerException.ToString());
                return "";

            }
        }
        static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        static Image ScaleByPercentS(Image imgPhoto, int Percent)
        {
            //double ratioX = (double)minDisplayX1 / (double)screen.Bounds.Width;
            //double ratioY = (double)minDisplayY1 / (double)screen.Bounds.Height;
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            //int destWidth = (int)(sourceWidth * nPercent);
            //int destHeight = (int)(sourceHeight * nPercent);
            int destWidth = 1366;
            int destHeight = 786;

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.CompositingQuality = CompositingQuality.HighQuality;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        static Image ConstrainProportions(Image imgPhoto, int Size, Dimensions Dimension)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            float nPercent = 0;

            switch (Dimension)
            {
                case Dimensions.Width:
                    nPercent = ((float)Size / (float)sourceWidth);
                    break;
                default:
                    nPercent = ((float)Size / (float)sourceHeight);
                    break;
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
            new Rectangle(destX, destY, destWidth, destHeight),
            new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
            GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);


            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = (int)((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = (int)((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Red);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        static Image Crop(Image imgPhoto, int Width, int Height, AnchorPosition Anchor)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (Anchor)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = (int)(Height - (sourceHeight * nPercent));
                        break;
                    default:
                        destY = (int)((Height - (sourceHeight * nPercent)) / 2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (Anchor)
                {
                    case AnchorPosition.Left:
                        destX = 0;
                        break;
                    case AnchorPosition.Right:
                        destX = (int)(Width - (sourceWidth * nPercent));
                        break;
                    default:
                        destX = (int)((Width - (sourceWidth * nPercent)) / 2);
                        break;
                }
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }


        static Image ScaleByPercentS2(Image imgPhoto, int Percent)
        {
            //double ratioX = (double)minDisplayX1 / (double)screen.Bounds.Width;
            //double ratioY = (double)minDisplayY1 / (double)screen.Bounds.Height;
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            double x = 1366;
            double y = 786;
            // Figure out the ratio
            double ratioX = (double)x / (double)sourceWidth;
            double ratioY = (double)y / (double)sourceHeight;

            int destWidth = (int)(sourceWidth * ratioX);
            int destHeight = (int)(sourceHeight * ratioY);
            //int destWidth = (int)(sourceWidth * nPercent);
            //int destHeight = (int)(sourceHeight * nPercent);
            //int destWidth = 1366;
            //int destHeight = 786;

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.CompositingQuality = CompositingQuality.HighQuality;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}
