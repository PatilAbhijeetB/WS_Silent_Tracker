using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using Emageia.Workshiftly.Domain.Concrete;
using Emageia.Workshiftly.Domain.Interface;
using Emageia.Workshiftly.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture
{
    public class ScreenshotUtility
    {
        #region Privete Fields

        private static int DEFAULT_SCREENSHOTS_PER_HOUR = 10;
        private static string SCREENSOHT_EXTENSION = "png";
        public static string RECORDED_MIME_TYPE = "image/png"; 

        private static int SCREENSHOT_WIDTH;
        private static int SCREENSHOT_HEIGHT;

        private static ScreenshotUtility INSTANCE;
        private static TimeslotDefinition timeslotDefinition;

        private ClientDataStoreDbContext _clientDbContext;

        //private Robot robot;
        //private Toolkit toolkit;
        private int screenshotPerHour;


        private static  double ACTIVE_WINDOW_SYNC_BATCH_SIZE = 20;
        private static  double SCREENSHOT_SYNC_BATCH_SIZE = 10;
        private static  double TASK_SYNC_BATCH_SIZE = 30;
        private static  double URL_SYNC_BATCH_SIZE = 10;

        #endregion

        #region Public Properties

        //private long CaptureTimestamp { get { return captureTimestamp; } set { captureTimestamp = value; } }
        //private Screenshot Screenshot { get { return screenshot; } set { screenshot = value; } }
        //private Boolean Commited { get { return Commited; } set { Commited = value; } }

        private static int minDisplayX = 0;
        private static int minDisplayY = 0;
        private static int totalCount = 0;
        #endregion


        #region constructor
        public ScreenshotUtility()
        {
            try
            {
                //get from database -> company define screenshot per hour
                int _screenshotsPerHour = CommonUtility.CompanyConfigurations.screenShotsPerHour == null ?
                                               0 : CommonUtility.CompanyConfigurations.screenShotsPerHour; //companyConfig.getNumberOfScreenshotsPerHour();

                screenshotPerHour = _screenshotsPerHour > 0 ? _screenshotsPerHour
                        : DEFAULT_SCREENSHOTS_PER_HOUR;
               // screenshotPerHour = 15;

                Int32 currentTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                timeslotDefinition = new TimeslotDefinition(currentTimestamp, screenshotPerHour);


                _clientDbContext = new ClientDataStoreDbContext();

            }
            catch(Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "ScreenshotUtility.cs ScreenshotUtility", ex.Message.ToString(), ex.InnerException.ToString());

            }

        }

              
        #endregion


        #region method

        public void getRunUtilityRunnable()
        {
            try
            {
              //  CommonUtility.LogWriteLine("ScreenShot Run Utility Runnable");
                Int32 currentTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

                if (timeslotDefinition.IsCompleted)
                {
                    CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs getRunUtilityRunnable", "ScreenShot timeslotDefinition.IsCompleted", "******");

                    //Todo compay configeration get getNumberOfScreenshotsPerHour
                    Int32 _screenshotPerHour = CommonUtility.CompanyConfigurations.screenShotsPerHour;
                    _screenshotPerHour = _screenshotPerHour != null && _screenshotPerHour > 0
                            ? _screenshotPerHour : DEFAULT_SCREENSHOTS_PER_HOUR;

                  //  _screenshotPerHour = 15;
                    timeslotDefinition = new TimeslotDefinition(currentTimestamp, _screenshotPerHour);

                }


                CapturePoint nextCapturePoint = timeslotDefinition.GetNextCapturePoint;
                Int32 nextCapturePointTimestamp = nextCapturePoint.CaptureTimestamp;

                if (nextCapturePointTimestamp <= currentTimestamp)
                {
                    Screenshot capturedScreenshot = captureCurrentScreenshot();
                    timeslotDefinition.commitScreenshotToCurrentCapturePoint(capturedScreenshot);
                }

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "ScreenshotUtility.cs getRunUtilityRunnable", ex.Message.ToString(), ex.InnerException.ToString());

            }
        }

        private Screenshot captureCurrentScreenshot()
        {


            //UserSession userSession = StateStorage.getCurrentState(StateName.USER_SESSION);
            //AppValidator.validateUserSession(userSession);
            CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs captureCurrentScreenshot", "ScreenShot >>>>>  captureCurrentScreenshot", "******");

            Screenshot screenshot = new Screenshot();
            screenshot.userId = CommonUtility.UserSessions.id;
            screenshot.companyId = CommonUtility.UserSessions.companyId;
            screenshot.mimeType = RECORDED_MIME_TYPE;


            Int32 currentTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            screenshot.timestamp = currentTimestamp;

            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");


            string UserId = CommonUtility.UserSessions.id;
            string CampanyId = CommonUtility.UserSessions.companyId;
            string fileName = String.Format("{0}/{1}/{2}/{3}", CampanyId, UserId, formattedDate, currentTimestamp);

            CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs captureCurrentScreenshot", "#### screenshot fileName >>> " + fileName, "******");
           
            screenshot.fileName = fileName;

            var currentBufferedImg = takeCurrentScreenshot();
            if(currentBufferedImg != null)
            {
                screenshot.Data = currentBufferedImg;

                return screenshot;
            }
            else
            {
                screenshot.Data = takeCurrentScreenshot();

                CommonUtility.LogWriteLines("Error", "ScreenshotUtility.cs captureCurrentScreenshot", "#### screenshot Data Nullllllllllllllllllllllllllll >>> " + fileName, "******");
                return screenshot;
            }

        }



        private string takeCurrentScreenshot() 
        {
            try
            {
                var periodTimeSpan = TimeSpan.FromSeconds(1);
                int i = 1;
                string root = @"C:\TimeTrack";

                root = CommonUtility.ImagePath;

                var allScreens = System.Windows.Forms.Screen.AllScreens;
                var screenCount = allScreens.Count();
                foreach (Screen screen in allScreens)
                {
                    var image = MultiScreen(screen, i, root);
                    i++;
                    if (image != null)
                    {
                        return image;
                    }

                    continue;
                    //else if(screenCount != i  )
                    //{

                    //}
                }
             //   var noi = await screen();
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
           
        }

        private async Task<string> screen()
        {
            return "";
        }
         
        private string MultiScreen(Screen screen ,int i,string root)
        {
            try
            {
              
                CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs MultiScreen", "ScreenShot >>>>>  take Current Screenshot", "*******");

                String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Graphics memoryGraphics = Graphics.FromImage(screenshot);
                memoryGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

                MemoryStream ms = new MemoryStream();
                screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                var details = ms.ToArray();

                try
                {
                    screenshot.Save(root + screen.DeviceName + filename);
                }catch (Exception esx)
                {

                }
                
                CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs MultiScreen", "ScreenShot >>>>>  take Current Screenshot", "******");

                return Convert.ToBase64String(details);

            }catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "ScreenshotUtility.cs MultiScreen", ex.Message.ToString(), ex.InnerException.ToString());
                return "";

            }
        }


        #endregion

        #region screenshot 

        private string takeCurrentScreenshot2()
        {
            try
            {
                List<Bitmap> bitmapImages = new List<Bitmap>();
                List<int> displayX = new List<int> { };
                List<int> displayY = new List<int> { };

                Screen[] allScreens = Screen.AllScreens;
                foreach (Screen screen in allScreens)
                {
                    Bitmap image = MultiScreen2(screen);
                    displayX.Add(screen.Bounds.Width);
                    displayY.Add(screen.Bounds.Height);
                    bitmapImages.Add(image);
                }
                totalCount = allScreens.Count();

                GetMinMaxDisplayXY2(displayX, displayY, totalCount);

               return CollectingImage(bitmapImages, minDisplayX, minDisplayY);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Bitmap MultiScreen2(Screen screen)
        {
            try
            {

                String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Graphics memoryGraphics = Graphics.FromImage(screenshot);
                memoryGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

                //  screenshot.Save(filename);


                return screenshot;


            }
            catch (Exception ex)
            {


                return null;
            }
        }


        private string CollectingImage(List<Bitmap> bitmapImages, int width, int height)
        {
            var bitmap2 = new Bitmap(width, height);
            var g = Graphics.FromImage(bitmap2);
            var localWidth = 0;

            foreach (Bitmap bitmap in bitmapImages)
            {
                Bitmap bitmapnew = null;
                if (bitmap != null)
                {
                    if (bitmap.Width > width)
                    {
                        String filenames = "Screen-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                        bitmapnew = ScaleByPercent(bitmap, width, height);
                        g.DrawImage(bitmapnew, localWidth, 0);
                        bitmapnew.Save(filenames);
                        localWidth += width;
                    }
                    else
                    {
                        g.DrawImage(bitmap, localWidth, 0);
                        localWidth += width;
                    }

                }
                continue;


            }

            MemoryStream ms = new MemoryStream();
            bitmap2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            var details = ms.ToArray();

            CommonUtility.LogWriteLines("Success", "ScreenshotUtility.cs MultiScreen", "ScreenShot >>>>>  take Current Screenshot", "******");

            String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
            bitmap2.Save(filename);
            return Convert.ToBase64String(details);
        }

        private static void GetMinMaxDisplayXY2(List<int> displayX, List<int> displayY, int totalCount)
        {
            try
            {
                minDisplayX = displayX.Min();
                minDisplayY = displayY.Min();

                //  minDisplayX1 = minDisplayX1 * totalCount;


                displayX.Clear();
                displayY.Clear();
            }
            catch (Exception ex)
            {

            }
        }

        static Bitmap ScaleByPercent(Bitmap imgPhoto, int x, int y)
        {
            //double ratioX = (double)minDisplayX1 / (double)screen.Bounds.Width;
            //double ratioY = (double)minDisplayY1 / (double)screen.Bounds.Height;
            //  float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            //double x = 1366;
            //double y = 786;
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

        #endregion



        #region screen
        private static int minDisplayX1 = 0;
        private static int minDisplayY1 = 0;
        private static int totalCount1 = 0;
        private static void CaptureDesktop2()
        {
            //minDisplayX1 = 0;
            //minDisplayY1 = 0;
            String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
            Rectangle desktopRect = GetDesktopBounds2();

            Bitmap bitmap = new Bitmap(minDisplayX1, minDisplayY1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                //  graphics.CopyFromScreen(desktopRect.Location, Point.Empty, bitmap.Size);
                graphics.CopyFromScreen(desktopRect.Location, Point.Empty, bitmap.Size);
            }

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            var details = ms.ToArray();


            //   bitmap.Save(filename, ImageFormat.Jpeg);
        }

        private static Rectangle GetDesktopBounds2()
        {
            try
            {
                totalCount1 = 0;
                System.Drawing.Rectangle result = new Rectangle();
                List<int> displayX = new List<int> { };
                List<int> displayY = new List<int> { };

                //Rectangle rectangle3 = Rectangle.Union(rectangle1, rectangle2);
                Screen[] allScreens = Screen.AllScreens;
                foreach (Screen scren in allScreens)
                {
                    System.Drawing.Rectangle bounds = scren.Bounds;
                    result = Rectangle.Union(result, bounds);
                    displayX.Add(scren.Bounds.Width);
                    displayY.Add(scren.Bounds.Height);
                    // saveImage(scren);
                }
                totalCount1 = allScreens.Count();
                GetMinMaxDisplayXY2(displayX, displayY, totalCount1);
                saveImage();
                return result;
            }
            catch (Exception)
            {
                return new Rectangle();

            }
        }

        private static void saveImage()
        {
            Screen[] allScreens = Screen.AllScreens;

            foreach (Screen screen in allScreens)
            {
                String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyy-hhmmss") + ".jpg";
                Image image = retriveImage(screen);



                var yourImage = resizeImage(image, new Size(minDisplayX1, minDisplayY1));

                Bitmap images = ResizeImage(image, minDisplayX1, minDisplayY1);


                image.Save(filename);


             
                System.Drawing.Image thumbnail =
                    new Bitmap(minDisplayX1, minDisplayY1);
                System.Drawing.Graphics graphic =
                             System.Drawing.Graphics.FromImage(thumbnail);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;


              
                //double ratioX = (double)minDisplayX1 / (double)screen.Bounds.Width;
                //double ratioY = (double)minDisplayY1 / (double)screen.Bounds.Height;
                double ratioX = 1;
                double ratioY = 1;


              
                int newWidth = Convert.ToInt32(screen.Bounds.Width * ratioX);
                int newHeight = Convert.ToInt32(screen.Bounds.Height * ratioY);


               
                int posX = Convert.ToInt32((minDisplayX1 - (screen.Bounds.Width * ratioX)) / 2);
                int posY = Convert.ToInt32((minDisplayY1 - (screen.Bounds.Height * ratioY)) / 2);

                graphic.Clear(Color.White);
                graphic.DrawImage(image, posX, posY, newWidth, newHeight);


           

                System.Drawing.Imaging.ImageCodecInfo[] info =
                                 ImageCodecInfo.GetImageEncoders();
                EncoderParameters encoderParameters;
                encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                                 100L);
                thumbnail.Save(filename, info[1],
                                 encoderParameters);

            }
        }


        private static Image retriveImage(Screen screen)
        {
            Bitmap screenshot = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics memoryGraphics = Graphics.FromImage(screenshot);
            memoryGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

            MemoryStream ms = new MemoryStream();
            screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            var details = ms.ToArray();

            return Image.FromStream(ms);
        }

        public static Bitmap resizeImage(Image imgToResize, Size size)
        {
            return (new Bitmap(imgToResize, size));
        }


        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }


        //private static void GetMinMaxDisplayXY2(List<int> displayX, List<int> displayY, int totalCount)
        //{
        //    try
        //    {
        //        minDisplayX1 = displayX.Min();
        //        minDisplayY1 = displayY.Min();

        //        // minDisplayX1 = minDisplayX1 * totalCount;


        //        displayX.Clear();
        //        displayY.Clear();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        #endregion
    }
}
