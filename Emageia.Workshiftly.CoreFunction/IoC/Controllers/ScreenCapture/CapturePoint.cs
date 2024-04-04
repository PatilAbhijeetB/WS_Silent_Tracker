using Emageia.Workshiftly.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture
{
    public class CapturePoint
    {

        #region Privete Fields

        private Int32 captureTimestamp;
        private Screenshot screenshot;
        private bool commited = false;
        private bool roundCompleted;

        #endregion

        #region Public Properties

        public Int32 CaptureTimestamp { get { return captureTimestamp; } set { captureTimestamp = value; } }
        public Screenshot Screenshot { get { return screenshot; } set { screenshot = value; } }
        public bool Commiteds { get { return Commited; } set { Commited = value; } }
        public bool Commited { get; set; } = false;
        public bool RoundCompleted { get { return roundCompleted; } set { roundCompleted = value; } }

        #endregion



        public CapturePoint(Int32 captureTimestamp)
        {
            this.captureTimestamp = captureTimestamp;
        }


        public int compareTo(long newTimestamp)
        {
            long difference = this.captureTimestamp - newTimestamp;
            return difference == 0 ? 0 : difference > 0 ? 1 : -1;
            
        }
       

    }
}
