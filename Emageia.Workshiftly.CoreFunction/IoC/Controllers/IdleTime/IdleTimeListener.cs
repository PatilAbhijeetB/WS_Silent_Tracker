using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ActiveWindow;
using Emageia.Workshiftly.CoreFunction.IoC.Controllers.ScreenCapture;
using Emageia.Workshiftly.CoreFunction.IoC.Service.SocketServer;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using Emageia.Workshiftly.Domain.Concrete;
using Emageia.Workshiftly.Entity;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Emageia.Workshiftly.CoreFunction.IoC.Controllers.IdleTime
{
    public class IdleTimeListener
    {
        public static Int32 idleTimeDifference;
        public static async void activeSlientTrackingHandler(Int32 currentIdleTime)
        {
            CommonUtility.IdledispatcherTimer.Stop();
            //CompanyConfiguration companyConfig = this.companyConfiguration.get();
            Int32 maxAllowedIdleTime = 3 * 60;

            Int32 idelTimeDiff = maxAllowedIdleTime - currentIdleTime;
            idleTimeDifference = idelTimeDiff;

            Int32 signBit = Math.Sign(idelTimeDiff);

            if (signBit <= 0.0)
            {
                try
                {
                    CommonUtility.ActiveWindoDispatcherTimer.Stop();
                    // recordWorkStatusLogsOnSlientMode(WorkStatusLog.WorkStatus.BREAK);
                    SlientModeWorkStatusActivator activatorService = new SlientModeWorkStatusActivator();
                    // activatorService.setPeriod(IDLE_TIME_LISTENER_INTERVAL);

                    Call(activatorService);
                    //Thread syncActiveWindow = new Thread(() => Call(activatorService));
                    //syncActiveWindow.Start();


                }
                catch (Exception ex)
                {
                    CommonUtility.IdledispatcherTimer.Start();
                }
            }
        }
        public static async void Call(SlientModeWorkStatusActivator activatorService)
        {
            try
            {
                CommonUtility.LogWriteLines("Sucess  START", "IdleTimeListener.CS activeSlientTrackingHandler ", "activeSlientTrackingHandler ----> Call", "*** WATCHING THE MOUSE KEY EVENT **");

               // Console.WriteLine("======================================================= activeSlientTrackingHandler)  ----> Call");

                var _activeTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _activeTimer.Tick += delegate (object sender, EventArgs e)
                {

                    if (!activatorService.callLister())
                    {

                        var name = "time click ";
                        
                    }
                    else
                    {
                        try
                        {

                            if (CommonUtility.IsIdleListener && CommonUtility.IsIdleDbWrite && CommonUtility.Status =="BREAK") {

                                CommonUtility.ActiveWindowUtilityFunction = new ActiveWindowUtility();
                                CommonUtility.ActiveWindoDispatcherTimer.Start();
                                CommonUtility.screenCaptureUtilityFunction = new ScreenshotUtility();
                                CommonUtility.ScreenCaptureUtility.Start();
                                

                                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                                {
                                    WorkStatusLog workStatusLog = new WorkStatusLog();

                                    workStatusLog.userId = CommonUtility.UserSessions.id;
                                    workStatusLog.deviceId = CommonUtility.LoggedDeviceSettings.deviceID; ;
                                    workStatusLog.userDate = DateTime.Now.ToString("yyy-MM-dd");
                                    workStatusLog.date = CommonUtility.StartOfDay();
                                    workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                                    workStatusLog.workStatus = "START";
                                    workStatusLog.isBreakAutomaticallyStart = false;
                                    workStatusLog.breakAutomaticallyStartDuration = 0;
                                    workStatusLog.isSynced = false;
                                    workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                                    //_clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                                    //_clientDbContext.SaveChangesAsync();
                                    CommonUtility.Status = "START";
                                    List<WorkStatusLog> WorkStatusLogsList = new List<WorkStatusLog>();
                                    WorkStatusLogsList.Add(workStatusLog);
                                    HttpRequestSocketCaller.shocketCallPostSyncWorkStatusLogs(WorkStatusLogsList);
                                }
                                CommonUtility.LogWriteLines("Success", "IdleTimeListener.CS activeSlientTrackingHandler", "Idle - Stop -------> workStatus = START  " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()), "**************");

                                CommonUtility.IsIdleDbWrite = false;

                                CommonUtility.IsIdleListener = false;

                                CommonUtility.IdledispatcherTimer.Start();
                                _activeTimer.Stop();
                            }


                            
                        }
                        catch (Exception ex)
                        {
                            CommonUtility.LogWriteLines("Error", "IdleTimeListener.CS activeSlientTrackingHandler ", "IDLE Error", ex.Message.ToString());

                        }

                    }

                };
                _activeTimer.Start();



            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "IdleTimeListener.CS activeSlientTrackingHandler ", "IDLE Error", ex.Message.ToString());

            }

        }

    }
}
