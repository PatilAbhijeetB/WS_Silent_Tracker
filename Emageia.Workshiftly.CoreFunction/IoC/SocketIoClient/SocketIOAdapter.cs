using Emageia.Workshiftly.CoreFunction.DataModels;
using Emageia.Workshiftly.CoreFunction.IoC.Service.Http;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using Emageia.Workshiftly.Domain.Concrete;
using Emageia.Workshiftly.Entity;
using Emageia.Workshiftly.Entity.HttpClientModel.activewindo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Planning.Targets;
using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Emageia.Workshiftly.CoreFunction.IoC.SocketIoClient
{
    public delegate void ShocketActiveWindowEventHandler(bool value);
    public class ActiveW
    {
        public string Object { get; set; }
        public CompanyConfiguration companyConfiguration1 { get; set; }
    }

    public class SocketIOAdapter
    {
        public event ShocketActiveWindowEventHandler banceChange;

        public SocketIO client { get; set; }
        public SocketIOAdapter() 
        {
            SocketInitalition();
        }

        private async void SocketInitalition()
        {
            var devId = 0;
            var deviceId = (CommonUtility.LoggedDeviceSettings.deviceID > 0) ? CommonUtility.LoggedDeviceSettings.deviceID.ToString() : devId.ToString();
           // var deviceId = devId.ToString();
            try
            {
                client = new SocketIO("https://portal.workshiftly.com/api", new SocketIOOptions

               // client = new SocketIO("https://staging-api.workshiftly.com/api", new SocketIOOptions
                {
                    Query = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", CommonUtility.UserSessions.authToken ),
                    new KeyValuePair<string, string>("userId", CommonUtility.UserSessions.id),
                    new KeyValuePair<string, string>("deviceId",deviceId)
                }
                });


                client.OnConnected += async (sender, e) =>
                {
                    Console.WriteLine("Socket Initalition");
                    socketIoAllListiner();

                };



                client.OnDisconnected += (sender, e) =>
                {
                    DisconnectedStatusUpdate();
                    Console.WriteLine("Socket Disconnected");
                };

                await client.ConnectAsync();
            }
            catch (Exception es)
            {

               // throw;
            }

            try
            {
                Thread syncActiveWindow = new Thread(() => HttpRequestCaller.syncActivityWindows());
                syncActiveWindow.Start();

                Thread callPostSyncWorkStatusLogs = new Thread(() => HttpRequestCaller.CallPostSyncWorkStatusLogs());
                callPostSyncWorkStatusLogs.Start();

                Thread capture = new Thread(() => HttpRequestCaller.syncScreenshots());
                capture.Start();
            }
            catch(Exception ex)
            {

            }

        }

        public static T Deserialize<T>(string json)
        {
            Newtonsoft.Json.JsonSerializer s = new Newtonsoft.Json.JsonSerializer();
            return s.Deserialize<T>(new JsonTextReader(new StringReader(json)));
        }

        private void socketIoAllListiner()
        {

            client.On("activeWindowData:Post", response =>
            {
                // banceChange(true);
                // You can print the returned data first to decide what to do next.
                // output: ["ok",{"id":1,"name":"tom"}]
                Console.WriteLine(response);
                //   string text = response.GetValue<string>();
                var getvaluea = response.GetValue();
                var returnData = JsonConvert.DeserializeObject<ActiveWindowReturn>(getvaluea.ToString());


            });

            client.On("workStatusLogData:Post", response =>
            {
                // You can print the returned data first to decide what to do next.
                // output: ["ok",{"id":1,"name":"tom"}]
                Console.WriteLine(response);
                // string text = response.GetValue<string>();

                //var returnData = JsonConvert.DeserializeObject<ActiveWindowReturn>(text);


            });
            //Update Company Configurations
            client.On("updateCompanyConfiguration:Push", response =>
            {

                Console.WriteLine(response);

            
            
                var getvaluea = response.GetValue();
              
                var companyConfigurationSocket = JsonConvert.DeserializeObject<CompanyConfiguration>(getvaluea.ToString());
            
               

                var getvalue = response.GetValue();
                var text = response.GetValue<CompanyConfiguration>();
                var responseData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, dynamic>>(response.GetValue());
                CompanyConfiguration companyConfiguration = new CompanyConfiguration();

                PropertyInfo[] properties = typeof(CompanyConfiguration).GetProperties();

                foreach (var property in properties)
                {
                    foreach(var r in responseData)
                    {
                        if (r.Value.ValueKind == JsonValueKind.String )
                        {
                             new CompanyConfiguration().GetType()
                            .GetProperty(r.Key.ToString())
                            .SetValue(companyConfiguration, r.Value.GetString());
                            //var stringValue = r.Value.GetString();
                        }
                        if(r.Value.ValueKind == JsonValueKind.Array)
                        {
                           // valget(r);
                            //JsonConvert.DeserializeObject(USER_SESSION);
                           // var noi = r.Value;
                           // var payments = r.Value.TryGetValue<IEnumerable<BreakReasons>>(r.Key.ToString());
                           // var m = JsonConvert.DeserializeObject<BreakReasons>(r.Value);
                           // new CompanyConfiguration().GetType()
                           //.GetProperty(r.Key.ToString())
                           //.SetValue(companyConfiguration, m);
                        }
                    }
                }


        
                string USER_SESSION_VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(companyConfigurationSocket);

                try
                 {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {

                        var count = _clientDbContext.LogAppDatas.Where(log => log.LogType == "COMPANY_CONFIGURATION").ToList().Count;
                        var logAppData = _clientDbContext.LogAppDatas.Where(log => log.LogType == "COMPANY_CONFIGURATION").FirstOrDefault();
                        if (count >0)
                        {
                            _clientDbContext.LogAppDatas.Remove(logAppData);
                            var appdata = new Entity.LogAppData()
                            {
                                LogType = "COMPANY_CONFIGURATION",
                                LogData = USER_SESSION_VALUE
                            };
                           
                            _clientDbContext.LogAppDatas.AddAsync(appdata);

                        }
                        else
                        {
                            
                        }

                    }
                }
                catch (Exception ex)
                {
                    CommonUtility.LogWriteLines("Error", "Main.xaml LogingCredentialsUpdate", "Loging Credential Check Error", ex.Message.ToString());

                }

            });


            //upate company profile details done
            client.On("updateCompanyProfile:Push", response =>
            {

                Console.WriteLine(response);



                var getvaluea = response.GetValue();

                var companyConfigurationSocket = JsonConvert.DeserializeObject<Company>(getvaluea.ToString());

                string COMPANY_SETTINGS_VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(companyConfigurationSocket);

                try
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {

                        var count = _clientDbContext.LogAppDatas.Where(log => log.LogType == "COMPANY_SETTINGS").ToList().Count;
                        var logAppData = _clientDbContext.LogAppDatas.Where(log => log.LogType == "COMPANY_SETTINGS").FirstOrDefault();
                        if (count > 0)
                        {
                            _clientDbContext.LogAppDatas.Remove(logAppData);
                            var appdata = new Entity.LogAppData()
                            {
                                LogType = "COMPANY_SETTINGS",
                                LogData = COMPANY_SETTINGS_VALUE
                            };

                            _clientDbContext.LogAppDatas.AddAsync(appdata);

                        }
                        else
                        {

                        }

                    }
                }
                catch (Exception ex)
                {
                    CommonUtility.LogWriteLines("Error", "Main.xaml LogingCredentialsUpdate", "Loging Credential Check Error", ex.Message.ToString());

                }


            });

            //update user profile details Done
            client.On("updateUserProfile:Push", response =>
            {
                
                Console.WriteLine(response);

                var getvaluea = response.GetValue();

                var userSessionSocket = JsonConvert.DeserializeObject<UserSessions>(getvaluea.ToString());

                string USER_SESSION_VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(userSessionSocket);

                try
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {

                        var count = _clientDbContext.LogAppDatas.Where(log => log.LogType == "USER_SESSION").ToList().Count;
                        var logAppData = _clientDbContext.LogAppDatas.Where(log => log.LogType == "USER_SESSION").FirstOrDefault();
                        if (count > 0)
                        {
                            _clientDbContext.LogAppDatas.Remove(logAppData);
                            var appdata = new Entity.LogAppData()
                            {
                                LogType = "USER_SESSION",
                                LogData = USER_SESSION_VALUE
                            };

                            _clientDbContext.LogAppDatas.AddAsync(appdata);

                        }
                        else
                        {

                        }

                    }
                }
                catch (Exception ex)
                {
                    CommonUtility.LogWriteLines("Error", "Main.xaml LogingCredentialsUpdate", "Loging Credential Check Error", ex.Message.ToString());

                }



            });


            // force Logout
            client.On("forceLogout:Push", response =>
            {
                          
                Console.WriteLine(response);
                string text = response.GetValue<string>();
                if (CommonUtility.LoggedDeviceSettings.deviceID == Convert.ToInt16(text))
                {
                    CommonUtility.ActiveWindoDispatcherTimer.Stop();
                    CommonUtility.ScreenCaptureUtility.Stop();
                    CommonUtility.IdledispatcherTimer.Stop();

                    deleteDatabaseCreadintial();
                }
              //  var dto = response.GetValue<TestDTO>(1);


            });
            
            // Update User Task
            client.On("updatedUserTask:Push", response =>
            {
                
          //      Console.WriteLine(response);

            //    Console.WriteLine(response);
                //string text = response.GetValue<string>();
                //if (CommonUtility.DeviceSettings.id == Convert.ToInt16(text))
                //{
                //    CommonUtility.ActiveWindoDispatcherTimer.Stop();
                //    CommonUtility.ScreenCaptureUtility.Stop();
                //    CommonUtility.IdledispatcherTimer.Stop();

                //    deleteDatabaseCreadintial();
                //}

                //   string text = response.GetValue<string>();


            });
            
            //Created UserTask
            client.On("createdUserTask:Push", response =>
            {
                
                Console.WriteLine(response);

              //  string text = response.GetValue<string>();

            });
            
            // Updated Logged In Device
            client.On("updateLoggedInDevices:Push", response =>
            {

                Console.WriteLine(response);
                //string text = response.GetValue();
                //if (CommonUtility.LoggedDeviceSettings.deviceID == Convert.ToInt16(text))
                //{
                //    CommonUtility.ActiveWindoDispatcherTimer.Stop();
                //    CommonUtility.ScreenCaptureUtility.Stop();
                //    CommonUtility.IdledispatcherTimer.Stop();

                //    deleteDatabaseCreadintial();
                //}
          
              //  string text = response.GetValue<string>();

                
            });

            // Update 
            client.On("updatUserWorkSchedules", response =>
            {
                
                Console.WriteLine(response);

                //   string text = response.GetValue<string>();


            });
        }

        public void valget(Dictionary<string, dynamic> data)
        {
            BreakReasons companyConfiguration = new BreakReasons();

            PropertyInfo[] properties = typeof(BreakReasons).GetProperties();

            foreach (var property in properties)
            {
                foreach (var r in data)
                {
                    if (r.Value.ValueKind == JsonValueKind.String)
                    {
                        new BreakReasons().GetType()
                       .GetProperty(r.Key.ToString())
                       .SetValue(companyConfiguration, r.Value.GetString());
                        //var stringValue = r.Value.GetString();
                    }
                    if (r.Value.ValueKind == JsonValueKind.Array)
                    {
                        //JsonConvert.DeserializeObject(USER_SESSION);
                      
                    }
                }
            }
        }

        public async void deleteDatabaseCreadintial()
        {
            try
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {
                    var logAppDatas = _clientDbContext.LogAppDatas.ToList();
                    _clientDbContext.LogAppDatas.RemoveRange(logAppDatas);
                    await _clientDbContext.SaveChangesAsync();
                    CommonUtility.HasLogging = false;

                }


                CommonUtility.UserSessions = null;
                CommonUtility.CompanySettings = null;
                CommonUtility.Company = null;
                CommonUtility.CompanyConfigurations = null;
                CommonUtility.DeviceSettings = null;
                DisconnectAsyncSocketIO();

            }
            catch (Exception)
            {

               /// throw;
            }
            
        }

        /// <summary>
        /// fire Dissconnect function on socket 
        /// </summary>
        public async void DisconnectAsyncSocketIO()
        {
            await client.DisconnectAsync();
        }

        /// <summary>
        /// Call ReconnectAsync function
        /// </summary>
        public async void ReconnectAsyncSockdtIo()
        {
            SocketInitalition();
        }

        public async void ActiveWindowPost(List<ActivietyWindows> activietyWindows)
        {
            try
            {
                var activietyWindowsset = new ResponseData<ActivietyWindows>();
                activietyWindowsset.requestData = activietyWindows;
                activietyWindowsset.userId = CommonUtility.UserSessions.id;


                if (activietyWindows.Count > 0)
                {
                  
                    try
                    {
                        var para = JsonConvert.SerializeObject(activietyWindowsset);
                        StringContent data = new StringContent(para, Encoding.UTF8, "application/json");
                        await client.EmitAsync("activeWindowData:Post", activietyWindowsset);


                        activeWindow(activietyWindows, true);

                    }
                    catch (Exception ex)
                    {

                        activeWindow(activietyWindows, false);

                       
                    }

                  
                }
              
            }
            catch (Exception ex)
            {
              
                activeWindow(activietyWindows, false);
            }
           
        }

        private async void activeWindow(List<ActivietyWindows> activietyWindows,bool synctype)
        {
            try
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {
                    if (activietyWindows.Count > 1)
                    {
                        foreach (var activ in activietyWindows)
                        {
                            activ.isSynced = synctype;
                           
                        }
                        await _clientDbContext.ActivietyWindows.AddRangeAsync(activietyWindows);
                        _clientDbContext.SaveChanges();
                    }
                    else
                    {
                        var activietyWindow = activietyWindows.FirstOrDefault();
                        activietyWindow.isSynced = synctype;
                        _clientDbContext.ActivietyWindows.Add(activietyWindow);
                        _clientDbContext.SaveChanges();

                    }

                }
            }
            catch (Exception) { 

            }
          
        }
        /// <summary>
        /// Work Status Logdata
        /// </summary>
        /// <param name="workstatus"></param>
        public async void WorkStatusLogDataPost(List<WorkStatusLog> workstatus)
        {
            try
            {
                var workstatusset = new ResponseData<WorkStatusLog>();
                workstatusset.requestData = workstatus;
                workstatusset.userId = CommonUtility.UserSessions.id;

                if (workstatus.Count > 0)
                {
                    try
                    {
                        var para = JsonConvert.SerializeObject(workstatus);
                        StringContent workstatusdata = new StringContent(para, Encoding.UTF8, "application/json");
                        await client.EmitAsync("workStatusLogData:Post", workstatusset);
                    }
                    catch (Exception ex)
                    {
                        WorkStatusLogDBWrite(workstatus, false);

                        // throw;
                    }

                }
                else
                {
                    WorkStatusLogDBWrite(workstatus, false);
                }
               
            }
            catch
            {
                WorkStatusLogDBWrite(workstatus, false);
            }
            
        }


        private async void WorkStatusLogDBWrite(List<WorkStatusLog> workstatus, bool synctype)
        {
            try
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {
                    try
                    {
                        if (workstatus.Count > 1)
                        {
                            foreach (var workstatusLog in workstatus)
                            {
                                workstatusLog.isSynced = synctype;

                            }
                            await _clientDbContext.WorkStatusLogs.AddRangeAsync(workstatus);
                            _clientDbContext.SaveChanges();
                        }
                        else
                        {
                            var workstatusLogs = workstatus.FirstOrDefault();
                            workstatusLogs.isSynced = synctype;
                            _clientDbContext.WorkStatusLogs.Add(workstatusLogs);
                            _clientDbContext.SaveChanges();

                        }
                    }
                    catch (Exception)
                    {


                    }


                }
            }
            catch (Exception)
            {

              //  throw;
            }
            
        }

        /// <summary>
        /// Screenshot Syncs 
        /// </summary>
        /// <param name="workstatus"></param>
        public async void ScreenshotPost(List<ScreenMetaHeder> screenMetaHeder)
        {
            var screenMetaHederset = new ResponseData<ScreenMetaHeder>();
            screenMetaHederset.requestData = screenMetaHeder;
            screenMetaHederset.userId = CommonUtility.UserSessions.id;
            if (screenMetaHeder.Count > 0)
            {
                try
                {
                    await client.EmitAsync("workStatusLogData:Post", screenMetaHederset);
                }
                catch (Exception)
                {

                   // throw;
                }

            }
        }

        private async void DisconnectedStatusUpdate()
        {
            try
            {
                if (IsLogeActiveUser())
                {
                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        WorkStatusLog workStatusLog = new WorkStatusLog();

                        workStatusLog.userId = CommonUtility.UserSessions.id;
                        workStatusLog.companyId = CommonUtility.UserSessions.companyId;
                        workStatusLog.deviceId = CommonUtility.DeviceSettings.id;
                        workStatusLog.userDate = DateTime.Now.ToString("yyyy-MM-dd");
                        workStatusLog.date = CommonUtility.StartOfDay();
                        workStatusLog.actionTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                        workStatusLog.workStatus = "DISCONNECT";
                        workStatusLog.isBreakAutomaticallyStart = false;
                        workStatusLog.breakAutomaticallyStartDuration = 0;
                        workStatusLog.isSynced = false;

                        await _clientDbContext.WorkStatusLogs.AddAsync(workStatusLog);
                        await _clientDbContext.SaveChangesAsync();

                        CommonUtility.LogWriteLine("///////////////////////////*************   DISCONNECT -------> Socket connection = DISCONNECT   " + Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
        }

        private async void UserLogoutCapture()
        {
            client.On("updateLoggedInDevices:Push", response =>
            {
               

                Console.WriteLine(response);


            });
        }
        private bool IsLogeActiveUser()
        {
            //if (CommonUtility.UserSessions != null && 
            //    CommonUtility.UserSessions?.id != null && CommonUtility.UserSessions?.companyId != null)
            //{
            //    return true;
            //}


            // return false;
            return CommonUtility.UserSessions?.id != null && CommonUtility.UserSessions?.companyId != null;
        }
    }
}
