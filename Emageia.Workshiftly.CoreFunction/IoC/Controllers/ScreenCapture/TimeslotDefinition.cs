using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture.ScreenShotLinkList;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using Emageia.Workshiftly.Domain.Concrete;
using Emageia.Workshiftly.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture
{
    public class TimeslotDefinition
    {
        #region Private Properties
        private static Int32 ONE_HOUR_DURATION = 3600;
        private static Int32 MAX_VALUE_SCREENSHOT_PER_HOUR = 15;
        private static Int32 CAPTURE_POINT_MULTIPLIER = 3;
        private static Int32 ACTIVE_THRESOLD_MULTIPLIER = 60;
        private static bool PERSIST_SCREEN_SHOTS_LOCAL_DISK;

        private Int32 startUnixTimestamp;
        private Int32 endUnixTimestamp;
        private Int32 screenshotsPerHour;
        private Int32 noOfCapturePoints;
        private Int32 activeThresold;

        private CapturePoint startCapturePoint;
        private CapturePoint endCapturePoint;
        private bool isCompleted;
        private LinkedList<CapturePoint> capturePoints = new LinkedList<CapturePoint>();
        LinkedLists<CapturePoint> capturePointsList = new LinkedLists<CapturePoint>();

        private CapturePoint nextCapturePoint;

       

        #endregion

        #region Constructor
        public TimeslotDefinition(Int32 startUnixTimestamp, Int32 screenshotsPerHour)
        {
            this.startUnixTimestamp = startUnixTimestamp;
            Int32 slotDuration = ONE_HOUR_DURATION / screenshotsPerHour;

            this.endUnixTimestamp = startUnixTimestamp + slotDuration;

            this.screenshotsPerHour = screenshotsPerHour;
            this.noOfCapturePoints = configureNoOfCapturePoints(screenshotsPerHour);

            this.activeThresold = configureActiveThresold(screenshotsPerHour);

            Int32 captureActiveStartpoint = startUnixTimestamp + activeThresold;
            this.startCapturePoint = new CapturePoint(captureActiveStartpoint);

            Int32 captureActiveEndPoint = endUnixTimestamp - activeThresold;
            this.endCapturePoint = new CapturePoint(captureActiveEndPoint);

            this.isCompleted = false;

            initializeCapturePoints();
        }
        #endregion

        #region Constructor calculation
        private int configureNoOfCapturePoints(Int32 screenshotsPerHour)
        {
            return (MAX_VALUE_SCREENSHOT_PER_HOUR / screenshotsPerHour) * CAPTURE_POINT_MULTIPLIER;
        }


        private Int32 configureActiveThresold(Int32 screenshotsPerHour)
        {
            Int32 factor = MAX_VALUE_SCREENSHOT_PER_HOUR / screenshotsPerHour;
            return (ACTIVE_THRESOLD_MULTIPLIER * factor);
        }
        #endregion

        #region Date Time converters

        /// <summary>
        /// Convert Unix time value to a DateTime object.
        /// </summary>
        /// <param name="unixtime">The Unix time stamp you want to convert to DateTime.</param>
        /// <returns>Returns a DateTime object that represents value of the Unix time.</returns>
        public static DateTime UnixTimeToDateTime(Int32 unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }



        /// <summary>
        /// Convert a DateTime to a unix timestamp
        /// </summary>+
        /// <param name="MyDateTime">The DateTime object to convert into a Unix Time</param>
        /// <returns></returns>
        public long DateTimeToUnix(DateTime MyDateTime)
        {
            TimeSpan timeSpan = MyDateTime - new DateTime(1970, 1, 1, 0, 0, 0);

            return (long)timeSpan.TotalSeconds;
        }

        #endregion

        #region Method
        public bool IsCompleted { get { return isCompleted; } }
        public CapturePoint GetNextCapturePoint { get { return nextCapturePoint; } }
        private void initializeCapturePoints()
        {
            try
            {
                Int32 noOfCapturePoints = this.noOfCapturePoints;
                Int32 captureActivatedTimestamp = this.startCapturePoint.CaptureTimestamp;
                Int32 captureInactivedTimestamp = this.endCapturePoint.CaptureTimestamp;
                Int32 capturedSeconds = captureInactivedTimestamp - captureActivatedTimestamp;

                capturePointsList.Add(this.startCapturePoint);
                capturePoints.AddFirst(this.startCapturePoint);
                capturePoints.AddLast(this.endCapturePoint);


                Int32 tempDuration = this.startCapturePoint.CaptureTimestamp;
                Int32 incrementUpperlimit = capturedSeconds / noOfCapturePoints;

                Random rnd = new Random();
                for (int step = 1; step <= noOfCapturePoints; step++)
                {

                    double currentIncrement = incrementUpperlimit * rnd.NextDouble();
                    Int32 capturingTimestamp = tempDuration + Convert.ToInt32(currentIncrement);
                    CapturePoint capturePoint = new CapturePoint(capturingTimestamp);
                    capturePoints.AddLast(capturePoint);
                    capturePointsList.Add(capturePoint);
                    tempDuration = tempDuration + incrementUpperlimit;

                }
                // CapturePoint filtered = capturePoints.Find((_CapturePoint) => _CapturePoint.)
                capturePointsList.Add(this.endCapturePoint);
                this.nextCapturePoint = this.startCapturePoint;
            }
            catch (Exception ex)
            {

                CommonUtility.LogWriteLines("Error", "TimeslotDefinition.CS    initializeCapturePoints ",ex.ToString(), "*****");

            }


        }



        public bool commitScreenshotToCurrentCapturePoint(Screenshot screenshot)
        {
            try
            {

                CommonUtility.LogWriteLine("ScreenShot >>>>>  commited Screenshot To Current Capture Point \n");
                CapturePoint currentCapturePoint = this.nextCapturePoint;
                currentCapturePoint.Screenshot = screenshot;
                var stats = this.startCapturePoint.CaptureTimestamp;
                var ens = this.endCapturePoint.CaptureTimestamp;

                if (currentCapturePoint.CaptureTimestamp == this.endCapturePoint.CaptureTimestamp)
                {
                    var response = finalizeTimeslot();
                    return response;
                }
                //else
                //{
                //    using (var _clientDbContext = new ClientDataStoreDbContext())
                //    {
                //        _clientDbContext.AddAsync(screenshot);
                //        _clientDbContext.SaveChangesAsync();
                //    }
                //}


                var currentCapturePointIndex = capturePointsList.IndexOf(currentCapturePoint);
                this.nextCapturePoint = (CapturePoint)capturePointsList.Get(currentCapturePointIndex + 1);


                // var n = capturePoints.
                //int nextIdx = this.capturePoints.indexOf(this.nextCapturePoint) + 1;
                //int nextIdxa = this.capturePoints.i
                //this.nextCapturePoint = this.capturePoints.get(nextIdx);
                //return new Response(false, StatusCode.SUCCESS, "Successfully committed screenshot to capture point");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

           
        }

        private Screenshot getResults()
        {

            //Shuffle 
            CommonUtility.LogWriteLine("ScreenShot >>>>>  finalizeTimeslot -->>>get Results");

            for (int i = 0; i < capturePointsList.Count; i++)
            {
                CapturePoint currentPoint = (CapturePoint)capturePointsList.Get(i);
                if (isValidResultPoint(currentPoint))
                {
                    CommonUtility.LogWriteLine("ScreenShot >>>>> ###> Shuffle <### finalizeTimeslot -->>>get Results");

                    return currentPoint.Screenshot;
                }
            }

            CapturePoint startedCapturePoint = (CapturePoint)this.capturePointsList.GetFirst();
            if (isValidResultPoint(startedCapturePoint))
            {
                CommonUtility.LogWriteLine("ScreenShot >>>>> ###> startedCapturePoint <### finalizeTimeslot -->>>get Results");
                return startedCapturePoint.Screenshot;
            }

            CommonUtility.LogWriteLine("ScreenShot >>>>> ###> endedCapturePoint <### finalizeTimeslot -->>>get Results \n \n");

            CapturePoint endedCapturePoint = (CapturePoint)this.capturePointsList.GetLast();
            return isValidResultPoint(endedCapturePoint) ? endedCapturePoint.Screenshot : null;
        }

        public CapturePoint GetNextPlayer(CapturePoint currentCapturePoint)
        {
           
            var curNode = capturePoints.Find(currentCapturePoint);
            curNode.Value.RoundCompleted = true;
           
            LinkedListNode<CapturePoint> nextNode = curNode.Next;
           
           
            nextNode = nextNode == null ? capturePoints.First : nextNode;

           
            while (curNode != nextNode)
            {
                
                if (!nextNode.Value.RoundCompleted)
                    break;
                
                nextNode = nextNode?.Next == null ? capturePoints.First : nextNode.Next;
            }
            //SetCurrentPlayer(nextNode.Value);
            //if(curNode != nextNode)
            //{
            //    return nextNode.Value;
            //}
            //nextNode = nextNode?.Next == null ? capturePoints.First : nextNode.Next;


            return nextNode.Value;
        }
        //public fIND
        private bool isValidResultPoint(CapturePoint capturePoint)
        {
            var noi = !capturePoint.Commited;
            return capturePoint != null && !capturePoint.Commited && capturePoint.Screenshot != null;
        }

       
        private Screenshot getResult()
        {
            foreach(CapturePoint currentPoint in capturePoints)
            {
                if (isValidResultPoint(currentPoint))
                {
                    return currentPoint.Screenshot;
                }
            }

            CapturePoint startedCapturePoint = this.capturePoints.First();
            if (isValidResultPoint(startedCapturePoint))
            {
                return startedCapturePoint.Screenshot;
            }

            CapturePoint endedCapturePoint = this.capturePoints.Last();
            return isValidResultPoint(endedCapturePoint) ? endedCapturePoint.Screenshot : null;
        }


        private bool finalizeTimeslot()
        {
            //Screenshot finalizedScreenshot;
            try
            {
                CommonUtility.LogWriteLine("ScreenShot >>>>>  finalizeTimeslot");
                this.isCompleted = true;
                Screenshot finalizedScreenshot = getResults();

                if (finalizedScreenshot == null)
                {
                    CommonUtility.LogWriteLine("ScreenShot >>>>>  finalizeTimeslot --- False");
                    return false; 
                        // return new Response(false, StatusCode.SUCCESS, "No screenshot captured for this slot");
                }

                CommonUtility.LogWriteLine("ScreenShot >>>>>  finalizeTimeslot --> ClientDataStoreDbContext >> stored start ");
                using (var _clientDbContext = new ClientDataStoreDbContext())
                {
                    _clientDbContext.AddAsync(finalizedScreenshot);
                    _clientDbContext.SaveChangesAsync();
                }
                CommonUtility.LogWriteLine("ScreenShot >>>>>  finalizeTimeslot --> ClientDataStoreDbContext >> stored end ");
                return true;


            }catch (Exception ex)
            {
                return false;
            }
            finally
            {
                

            }

        }
           
   
        #endregion
    }
}



